using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public bool IsHoldingWeapon { get; private set; } = false;

    [SerializeField] GameObject Barrel;
    [SerializeField] PlayerVisualController playerVisualController;

    [SerializeField] List<PlayerWeapon> OwnedWeapons = new List<PlayerWeapon>();
    PlayerWeapon currentWeapon = null;
    int currentWeaponIndex = -1;

    Dictionary<string, float> dic_nextAvaibleTime = new Dictionary<string, float>();
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) SetIsHoldingWeapon(!IsHoldingWeapon);
        if (Input.GetKeyDown(KeyCode.K)) TryUseWeapon();
        if (Input.GetKeyDown(KeyCode.E)) SwitchToNextWeapon();
        if (Input.GetKeyDown(KeyCode.Q)) SwitchToPreviousWeapon();
    }
    void Start()
    {
        if(OwnedWeapons.Count > 0)
        {
            SwitchToWeapon(0);
        }

        ReinitalizeCooldownDictionary();
        SetIsHoldingWeapon(IsHoldingWeapon);
    }

    bool TryUseWeapon()
    {
        if (IsHoldingWeapon == false) return false;

        if (currentWeapon == null) return false;
        if (dic_nextAvaibleTime.TryGetValue(currentWeapon.ID, out float nextAvaibleTime) == false) return false;
        if (nextAvaibleTime > Time.time) return false;

        Debug.Log($"{currentWeapon.name} dýkþýn dýkþýn diyor");
        dic_nextAvaibleTime[currentWeapon.ID] = Time.time + currentWeapon.FireCooldown;
        StartCoroutine(BlinkAnimation(Barrel, false));

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
        if (weapon.ID == "[UNASSIGNED]") Debug.LogError("Cannot acquire a weapon with unassigned ID");
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
}
