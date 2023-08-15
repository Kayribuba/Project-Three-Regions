using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Player : Projectile_Base
{
    internal override void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInitalized == false) return;
        if (collision.gameObject.tag == "Player") return;
        if (collision.gameObject.tag == gameObject.tag) return;

        base.OnTriggerEnter2D(collision);
    }
}
