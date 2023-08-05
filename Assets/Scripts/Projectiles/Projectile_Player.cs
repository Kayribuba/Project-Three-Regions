using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Player : Projectile_Base
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInitalized == false) return;
        if (collision.gameObject.tag == "Player") return;
        if (collision.gameObject.tag == gameObject.tag) return;

        if(DestroyEffect != null)
        {
            Instantiate(DestroyEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
