using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Walker : Entity
{
    enum WalkerBehaviour { Walking, Waiting, Attacking }

    [Header("Design")]
    [SerializeField] BulletGroup[] bullets;
    [SerializeField] float pauseTime = 1f;
    [SerializeField] float rememberTime = 1f;
    [SerializeField] float sightRadius = 5f;
    [SerializeField] float barrelDistance = .5f;

    [Header("Visuals")]
    [SerializeField] GameObject WarnedEffect;
    [SerializeField] Transform WarnedEffectPoint;

    [Header("Reference")]
    [SerializeField] LayerMask SightObstructionLayers = 1 << 6 | 1 << 7;
    [SerializeField] Transform downCheck;
    [SerializeField] Transform sideCheck;
    [SerializeField] float downCheckRange = .5f;
    [SerializeField] Vector2 sideCheckSize = Vector2.one;

    GameObject Player;
    int bulletIndex = 0;

    Vector2 _playerPos
    {
        get
        {
            if (Player == null)
            {
                Player = GameManager.Instance.Player;

                if (Player == null) return Vector2.zero;
            }
            return Player.transform.position;
        }
    }

    WalkerBehaviour currentBeh = WalkerBehaviour.Walking;
    Vector2 direction => isFacingLeft ? Vector2.left : Vector2.right;
    bool isFacingLeft;

    float nextAttackTime = -1;
    float forgetPlayerAt = -1;
    bool isRememberingPlayer => forgetPlayerAt > Time.time;

    public override void Start()
    {
        base.Start();

        Player = GameManager.Instance.Player;
    }

    internal override void UpdateBehaviour()
    {
        TrySeePlayer();

        //if(b_inputEnabled)
        //{
            switch (currentBeh)
            {
                case WalkerBehaviour.Walking:
                    Walk();
                    break;
                case WalkerBehaviour.Waiting:
                    Wait();
                    break;
                case WalkerBehaviour.Attacking:
                    Attack();
                    break;
            }
        //}
    }

    bool TrySeePlayer()
    {
        Vector2 playerPos = _playerPos;
        Vector2 transformPos = transform.position;

        if (Vector2.Distance(transformPos, playerPos) > sightRadius) return false;

        Vector2 sightDir = playerPos - transformPos;
        sightDir.Normalize();

        RaycastHit2D hit = Physics2D.Raycast(transformPos, sightDir, sightRadius, SightObstructionLayers);

        if (hit.collider == null) return false;

        if (hit.collider.gameObject.tag == "Player")
        {
            if (isRememberingPlayer == false)//First time entering
            {
                SwitchBehaviour(WalkerBehaviour.Attacking);
            }

            forgetPlayerAt = Time.time + rememberTime;
            return true;
        }

        return false;
    }

    void Walk()
    {
        GroundCheck(out bool hitDown, out bool hitSide);

        if (hitDown == false)
        {
            SwitchBehaviour(WalkerBehaviour.Waiting);
            return;
        }

        if (hitSide == true)
        {
            Turn();
        }

        Vector2 targetVel = rb.velocity;
        targetVel.x = b_speed * direction.x;

        rb.velocity = targetVel;
    }

    float waitExitTime = float.MaxValue;

    void Wait()
    {
        if (waitExitTime <= Time.time)
        {
            Turn();
            SwitchBehaviour(WalkerBehaviour.Walking);
        }
    }

    void Attack()
    {
        Vector2 transformPos = transform.position;
        Vector2 sightDir = _playerPos - transformPos;
        sightDir.Normalize();

        if (nextAttackTime <= Time.time)//Attack time
        {
            Vector2 barrelPos = transformPos + (sightDir * barrelDistance);

            if (Vector2.Dot(sightDir, direction) < 0) Turn();


            for (int i = 0; i < bullets[bulletIndex].Bullets.Length; i++)
            {
                Bullet bullet = bullets[bulletIndex].Bullets[i];
                if (bullet.Projectile == null) continue;

                Vector2 newDir = Quaternion.AngleAxis(bullet.TurnDegree, Vector3.forward) * sightDir;
                Vector2 newPos = barrelPos + (Vector2)(Quaternion.AngleAxis(Vector2.Angle(Vector2.right, sightDir), Vector3.forward) * bullet.Offset);

                GameObject instProj = Instantiate(bullet.Projectile, newPos, Quaternion.identity);
                if (instProj.TryGetComponent(out Projectile outProj))
                {
                    outProj.Initalize(newDir, false, b_damage, GLOBAL.EnemyIgnoreTags);
                }
            }

            nextAttackTime = Time.time + bullets[bulletIndex].Cooldown;
            IncrementBulletIndex();
        }

        if (isRememberingPlayer == false)
        {
            SwitchBehaviour(WalkerBehaviour.Walking);
        }
    }

    void IncrementBulletIndex() => bulletIndex = (bulletIndex + 1) % bullets.Length;

    void SwitchBehaviour(WalkerBehaviour switchTo)
    {
        switch (switchTo)//Entering behaviour
        {
            case WalkerBehaviour.Walking:

                break;
            case WalkerBehaviour.Waiting:
                waitExitTime = Time.time + pauseTime;
                break;
            case WalkerBehaviour.Attacking:
                if (WarnedEffect != null)
                {
                    if (WarnedEffectPoint != null)
                    { Instantiate(WarnedEffect, WarnedEffectPoint.transform.position, Quaternion.identity, WarnedEffectPoint.transform); }
                    else
                    {
                        Instantiate(WarnedEffect, transform.position, Quaternion.identity, transform);
                    }
                }
                break;
        }

        switch (currentBeh)//Exiting behaviour
        {
            case WalkerBehaviour.Walking:
                rb.velocity = rb.velocity * new Vector2(0, 1);
                break;
            case WalkerBehaviour.Waiting:
                waitExitTime = float.MaxValue;
                break;
            case WalkerBehaviour.Attacking:

                break;
        }

        currentBeh = switchTo;
    }

    void GroundCheck(out bool hitDown, out bool hitSide)
    {
        int groundLayer = GLOBAL.GetLayerAt(6);

        hitDown = Physics2D.RaycastAll(downCheck.position, Vector2.down, downCheckRange, groundLayer).Length > 0;
        hitSide = Physics2D.OverlapBoxAll(sideCheck.position, sideCheckSize, 0, groundLayer).Length > 0;
    }
    void Turn()
    {
        isFacingLeft = !isFacingLeft;
        transform.localScale *= new Vector2(-1, 1);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(downCheck.position, (Vector2)downCheck.position + Vector2.down * downCheckRange);
        Gizmos.DrawWireCube(sideCheck.position, sideCheckSize);

        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
}