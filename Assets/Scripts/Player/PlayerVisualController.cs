using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer weaponSR;
    [SerializeField] GameObject VisualGO;
    [SerializeField] GameObject WeaponVisualGO;

    float velocity = 0;
    bool isGrounded = false;
    bool isHoldingWeapon = false;


    string currentAnimState;
    const string PlayerIdle = "Player_Idle";
    const string PlayerIdleGun = "Player_GunIdle";
    const string PlayerWalk = "Player_Walk";
    const string PlayerWalkGun = "Player_GunWalk";
    const string PlayerJump = "Player_Jump";
    const string PlayerJumpGun = "Player_GunJump";

    public void SetIsGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
        EvaluateAnimState();
    }
    public void SetVelocity(float velocity)
    {
        this.velocity = velocity;
        EvaluateAnimState();
    }
    public void SetHoldingWeapon(bool isHoldingWeapon)
    {
        this.isHoldingWeapon = isHoldingWeapon;

        WeaponVisualGO.SetActive(isHoldingWeapon);

        EvaluateAnimState();
    }
    public void SetWeaponSprite(Sprite weaponSprite)
    {
        if (weaponSR == null) return;

        weaponSR.sprite = weaponSprite;
    }
    public void SetWeaponSprite(PlayerWeapon playerWeapon)
    {
        if (weaponSR == null) return;

        weaponSR.sprite = playerWeapon.WeaponSprite;
    }


    void EvaluateAnimState()
    {
        string targetState = "";

        if(isGrounded)
        {
            if(Mathf.Abs(velocity) > .1f)
            {
                targetState = isHoldingWeapon ? PlayerWalkGun : PlayerWalk;
            }
            else
            {
                targetState = isHoldingWeapon ? PlayerIdleGun : PlayerIdle;
            }
        }
        else
        {
            targetState = isHoldingWeapon ? PlayerJumpGun : PlayerJump;
        }

        ChangeAnimationState(targetState);
    }
    void ChangeAnimationState(string setTo)
    {
        if (setTo == "") return;
        if (setTo == currentAnimState) return;

        animator.Play(setTo);

        currentAnimState = setTo;
    }
}
