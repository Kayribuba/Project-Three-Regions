using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public bool IsPlayerProjectile { get; private set; }

    [SerializeField]  GameObject DestroyEffect;
    [SerializeField]  float _damageMultiplier = 1;
    [SerializeField]  float _speedMultiplier = 1;
    [SerializeField]  float lifeTime = 10;

     float Damage => _damage * _damageMultiplier;
     float _damage = 0;
     Rigidbody2D rb;
     Collider2D col;
     bool isInitalized = false;
     string[] TagsToIgnore = null;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
    public void Initalize(Vector2 direction, bool isPlayerProjectile, float damage, string[] tagsToIgnore = null)
    {
        if (isInitalized) return;

        IsPlayerProjectile = isPlayerProjectile;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.velocity = direction.normalized * GLOBAL.ProjectileSpeed * _speedMultiplier;

        col.isTrigger = true;

        if (tagsToIgnore != null)
        {
            TagsToIgnore = tagsToIgnore;
        }

        _damage = damage;

        isInitalized = true;
    }

     void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInitalized == false) return;
        if (collision.isTrigger) return;

        if (TagsToIgnore != null)
        {
            foreach (string t in TagsToIgnore)
            {
                if (collision.gameObject.tag == t)
                {
                    return;
                }
            }
        }

        //if(collision.TryGetComponent(out Projectile_Base P))
        //{
        //    if (P.IsPlayerProjectile == IsPlayerProjectile) return;
        //}


        if (collision.TryGetComponent(out Entity entity))
        {
            entity.RemoveHealth(Damage);
        }

        if (DestroyEffect != null)
        {
            Instantiate(DestroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
