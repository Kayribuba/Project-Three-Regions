using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] PlayerWeapon WeaponToPickUp;
    [SerializeField] GameObject PickupVFX;

    void PickWeapon(PlayerWeaponController PWController)
    {
        if (WeaponToPickUp == null)
        {
            Debug.LogError("Weapon pickup can not be empty");
            Destroy(gameObject);
            return;
        }

        PWController.AcquireWeapon(WeaponToPickUp);

        if (PickupVFX != null)
        { Instantiate(PickupVFX, transform.position, Quaternion.identity); }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;

        if(go.tag == "Player")
        {
            if (go.TryGetComponent(out PlayerWeaponController pwc))
            {
                PickWeapon(pwc);
            }
        }
    }
}
