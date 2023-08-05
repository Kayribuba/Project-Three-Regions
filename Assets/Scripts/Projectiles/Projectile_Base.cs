using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public abstract class Projectile_Base : MonoBehaviour
{
    [SerializeField] internal GameObject DestroyEffect;
    [SerializeField] internal float _damageMultiplier = 1;
    [SerializeField] internal float _speedMultiplier = 1;
    [SerializeField] internal float lifeTime = 10;

    internal PlayerWeapon weaponData;
    internal Rigidbody2D rb;
    internal Collider2D col;
    internal bool isInitalized = false;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    public virtual void Initalize(Vector2 direction, PlayerWeapon weaponData, string tagToGive = null)
    {
        if (isInitalized) return;

        Debug.Log(direction);

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.velocity = direction.normalized * GLOBAL.ProjectileSpeed * _speedMultiplier;

        col.isTrigger = true;

        this.weaponData = weaponData;

        if (tagToGive != null)
            gameObject.tag = tagToGive;

        isInitalized = true;
    }
}
