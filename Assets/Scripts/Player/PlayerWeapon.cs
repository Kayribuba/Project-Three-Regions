using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerWeapon")]
public class PlayerWeapon : ScriptableObject
{
    [SerializeField] string _ID = "[UNASSIGNED]";
    [SerializeField] string _displayName = "[UNASSIGNED]";
    [SerializeField] string _weaponDescription = "[UNASSIGNED]";
    [SerializeField] float _damage = 1;
    [SerializeField] float _fireCooldown = 1;
    [SerializeField] FireType _firingType = FireType.Single;
    [Tooltip("Only used if using burst mode. Determines how many bullets will be shot in one burst")]
    [SerializeField] int _burstAmmoCount = 2;
    [Tooltip("Only used if using burst mode. Determines how long will the cooldown be when a burst is done or fire key is unpressed")]
    [SerializeField] float _burstCooldown = 2;

    [Tooltip("Every pixel in your sprite is 0.041666... in length :(")]
    [SerializeField] Vector2 _barrelOffset = new Vector2(.5f, 0);
    [SerializeField] Sprite _weaponSprite;
    [SerializeField] GameObject _projectile;

    public string ID => _ID;
    public string DisplayName => _displayName;
    public string WeaponDescription => _weaponDescription;
    public float Damage => _damage;
    public float FireCooldown => _fireCooldown;
    public FireType FiringType => _firingType;
    public int BurstAmmoCount => _burstAmmoCount;
    public float BurstCooldown => _burstCooldown;
    public Vector2 BarrelOffset => _barrelOffset;
    public Sprite WeaponSprite => _weaponSprite;
    public GameObject Projectile => _projectile;
}
public enum FireType { Single, Burst, Automatic }