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
    [SerializeField] Vector2 _barrelOffset = new Vector2(.5f, 0);
    [SerializeField] Sprite _weaponSprite;
    [SerializeField] GameObject _projectile;

    public string ID => _ID;
    public string DisplayName => _displayName;
    public string WeaponDescription => _weaponDescription;
    public float Damage => _damage;
    public float FireCooldown => _fireCooldown;
    public Vector2 BarrelOffset => _barrelOffset;
    public Sprite WeaponSprite => _weaponSprite;
    public GameObject Projectile => _projectile;
}
