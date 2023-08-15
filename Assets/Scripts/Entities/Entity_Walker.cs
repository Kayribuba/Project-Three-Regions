using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Walker : Entity
{
    [SerializeField] Transform downCheck;
    [SerializeField] Transform sideCheck;
    [SerializeField] float downCheckRange = .5f;
    [SerializeField] Vector2 sideCheckSize = Vector2.one;
    [SerializeField] float pauseTime = 1f;

    WalkerBehaviour currentBeh = WalkerBehaviour.Walking;
    Vector2 direction => isFacingLeft ? Vector2.left : Vector2.right;
    bool isFacingLeft;

    internal override void UpdateBehaviour()
    {
        switch(currentBeh)
        {
            case WalkerBehaviour.Walking:
                Walking();
                break;
            case WalkerBehaviour.Waiting:
                Waiting();
                break;
            case WalkerBehaviour.Attacking:
                Attacking();
                break;
        }
    }

    void Walking()
    {
        Debug.Log("walk");

        GroundCheck(out bool hitDown, out bool hitSide);

        if(hitDown == false)
        {
            SwitchBehaviour(WalkerBehaviour.Waiting);
            return;
        }

        if (hitSide == true)
        {
            Turn();
        }

        Vector2 targetVel = rb.velocity;
        targetVel.x = _speed * direction.x;

        rb.velocity = targetVel;
    }

    float waitExitTime = float.MaxValue;
    void Waiting()
    {
        if(waitExitTime <= Time.time)
        {
            Turn();
            SwitchBehaviour(WalkerBehaviour.Walking);
        }
    }
    void Attacking()
    {
        Debug.Log("attack");
    }

    void SwitchBehaviour(WalkerBehaviour switchTo)
    {
        switch (switchTo)//Entering behaviour
        {
            case WalkerBehaviour.Walking:

                break;
            case WalkerBehaviour.Waiting:
                rb.velocity = rb.velocity * new Vector2(0, 1);
                waitExitTime = Time.time + pauseTime;
                break;
            case WalkerBehaviour.Attacking:

                break;
        }

        switch (currentBeh)//Exiting behaviour
        {
            case WalkerBehaviour.Walking:
                
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
    }

    enum WalkerBehaviour { Walking, Waiting, Attacking}
}