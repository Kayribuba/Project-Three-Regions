using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerWeaponController : MonoBehaviour
{
    public UnityEvent<PlayerWeapon> OnWeaponChanged;
    public bool IsHoldingWeapon { get; private set; } = false;

    [SerializeField] GameObject Barrel;
    [SerializeField] PlayerVisualController playerVisualController;
    [SerializeField] PlayerController playerController;

    [SerializeField] List<PlayerWeapon> OwnedWeapons = new List<PlayerWeapon>();
    PlayerWeapon currentWeapon = null;
    int currentWeaponIndex = -1;

    Dictionary<string, float> dic_nextAvaibleTime = new Dictionary<string, float>();

    Vector2 FireDirection => GetFiringDireciton();

    bool PressedFireButton = false;
    bool HoldingFireButton = false;
    bool BurstOver = false;
    bool BurstHasRefired = true;
    int FireIteration = 0;

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Tab)) SetIsHoldingWeapon(!IsHoldingWeapon);
        if (Input.GetButtonDown("WeaponNext")) SwitchToNextWeapon();
        if (Input.GetButtonDown("WeaponPrevious")) SwitchToPreviousWeapon();

        if (Input.GetButtonDown("Fire"))
        {
            PressedFireButton = true;
            HoldingFireButton = true;
        }
        if (Input.GetButtonUp("Fire"))
        {
            HoldingFireButton = false;
            FireIteration = 0;

            if (IsHoldingWeapon && currentWeapon.FiringType == FireType.Burst && BurstHasRefired && BurstOver == false)
            {
                dic_nextAvaibleTime[currentWeapon.ID] = Time.time + currentWeapon.BurstCooldown;
            }

            BurstHasRefired = false;
            BurstOver = false;
        }

        if (IsHoldingWeapon)
        {
            if (PressedFireButton ||
                (HoldingFireButton && currentWeapon.FiringType == FireType.Automatic) ||
                (HoldingFireButton && currentWeapon.FiringType == FireType.Burst && BurstOver == false && (FireIteration != 0 || PressedFireButton)))
            {
                bool didFiredWeapon = TryUseWeapon();

                if (currentWeapon.FiringType == FireType.Burst && didFiredWeapon)
                {
                    FireIteration++;

                    if (FireIteration >= currentWeapon.BurstAmmoCount)
                    {
                        dic_nextAvaibleTime[currentWeapon.ID] = Time.time + currentWeapon.BurstCooldown;
                        BurstOver = true;
                    }
                }
            } 
        }

        PressedFireButton = false;
    }
    void Start()
    {
        if(OwnedWeapons.Count > 0)
        {
            SwitchToWeapon(0);
        }

        ReinitalizeCooldownDictionary();
    }

    bool TryUseWeapon()
    {
        if (IsHoldingWeapon == false) return false;

        if (currentWeapon == null) return false;
        if (dic_nextAvaibleTime.TryGetValue(currentWeapon.ID, out float nextAvaibleTime) == false) return false;
        if (nextAvaibleTime > Time.time) return false;

        if(currentWeapon.Projectile != null)
        {
            Quaternion targetRotation = GetDirectionAsRotation();
            GameObject instProjectile = Instantiate(currentWeapon.Projectile, Barrel.transform.position, targetRotation);

            if(instProjectile.TryGetComponent(out Projectile outProjectile))
            {
                outProjectile.Initalize(FireDirection, true, currentWeapon.Damage, new string[] { "Player" });
            }
        }

        dic_nextAvaibleTime[currentWeapon.ID] = Time.time + currentWeapon.FireCooldown;
        BurstHasRefired = true;

        return true;
    }

    IEnumerator BlinkAnimation(GameObject go, bool endWithDisabled = true)
    {
        go.SetActive(endWithDisabled);
        yield return new WaitForSeconds(.1f);
        go.SetActive(!endWithDisabled);
    }

    public bool WeaponOwned(PlayerWeapon weapon)
    {
        foreach (PlayerWeapon pw in OwnedWeapons)
        {
            if (weapon.ID == pw.ID)
            {
                return true;
            }
        }
        return false;
    }
    public bool WeaponOwned(PlayerWeapon weapon, out int index)
    {
        for (int i = 0; i < OwnedWeapons.Count; i++)
        {
            if (weapon.ID == OwnedWeapons[i].ID)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }
    public void AcquireWeapon(PlayerWeapon weapon)
    {
        if (weapon.ID == GLOBAL.UnnasignedString) Debug.LogError("Cannot acquire a weapon with unassigned ID");
        if (WeaponOwned(weapon)) return;

        OwnedWeapons.Add(weapon);
        dic_nextAvaibleTime.Add(weapon.ID, -1);

        if(currentWeapon == null)
        {
            SwitchToWeapon(0);
        }
    }
    public bool SwitchToWeapon(PlayerWeapon weapon)
    {
        if (WeaponOwned(weapon, out int outIndex) == false) return false;

        return SwitchToWeapon(outIndex);
    }
    public bool SwitchToWeapon(int index)
    {
        if (IndexIsValid(index) == false) return false;

        currentWeaponIndex = index;
        currentWeapon = OwnedWeapons[index];
        Barrel.transform.localPosition = currentWeapon.BarrelOffset;
        playerVisualController.SetWeaponSprite(currentWeapon.WeaponSprite);
        SetIsHoldingWeapon(true);

        OnWeaponChanged?.Invoke(currentWeapon);

        return true;
    }
    public bool IndexIsValid(int index) => OwnedWeapons.Count > index && index >= 0;
    public void SwitchToNextWeapon()
    {
        if (IsHoldingWeapon == false) return;

        int index = currentWeaponIndex + 1;

        if (IndexIsValid(index)) SwitchToWeapon(index);
        else SwitchToWeapon(0);
    }
    public void SwitchToPreviousWeapon()
    {
        if (IsHoldingWeapon == false) return;

        int index = currentWeaponIndex - 1;

        if (IndexIsValid(index)) SwitchToWeapon(index);
        else SwitchToWeapon(OwnedWeapons.Count - 1);
    }
    public void SetIsHoldingWeapon(bool setTo)
    {
        IsHoldingWeapon = setTo;
        playerVisualController?.SetHoldingWeapon(setTo);
    }
    public void ReinitalizeCooldownDictionary()
    {
        dic_nextAvaibleTime = new Dictionary<string, float>();

        foreach(PlayerWeapon pw in OwnedWeapons)
        {
            dic_nextAvaibleTime[pw.ID] = -1;
        }
    }

    Vector2 GetFiringDireciton()
    {
        if (playerController == null) return Vector2.zero;

        Vector2 direction = Vector2.zero;

        direction.x = playerController.gameObject.transform.localScale.x;
        if (direction.x > 0) direction.x = 1;
        else direction.x = -1;

        direction.y = 0;

        return direction;
    }
    Quaternion GetDirectionAsRotation()
    {
        Vector2 dir = FireDirection;

        if (dir.x == 1) return Quaternion.Euler(0, 0, 0);
        else return Quaternion.Euler(0, 0, 180);
    }
}
