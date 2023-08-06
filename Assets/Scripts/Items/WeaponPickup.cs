using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] PlayerWeapon WeaponToPickUp;

    void PickWeapon()
    {
        if(WeaponToPickUp == null)
        {
            Debug.LogError("Weapon pickup can not be empty");
            Destroy(gameObject);
            return;
        }


    }
}
