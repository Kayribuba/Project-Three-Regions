using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageCollider : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] float knockbackForce = 1;

    void DamagePlayer(Entity e)
    {
        //Vector2 flyDirection = e.transform.position - transform.position;
        //flyDirection.Normalize();

        //e.KnockBack(flyDirection, knockbackForce);
        e.RemoveHealth(damage);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(collision.TryGetComponent(out Entity e))
            {
                DamagePlayer(e);
            }
        }
    }
}
