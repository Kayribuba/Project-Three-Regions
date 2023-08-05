using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualController : MonoBehaviour
{
    [SerializeField] Animator[] animators;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject VisualGO;
    [SerializeField] GameObject WeaponVisualGO;

    [SerializeField] Animation anim;

    public void SetIsGrounded(bool isGrounded)
    {
        foreach(Animator a in animators)
        {
            a.SetBool("grounded", isGrounded);
        }
    }
    public void SetVelocity(float velocity)
    {
        foreach (Animator a in animators)
        {
            a.SetFloat("velocity", Mathf.Abs(velocity));
        }
    }
    public void SetHoldingWeapon(bool isHoldingWeapon)
    {
        foreach (Animator a in animators)
        {
            a.SetBool("isHoldingWeapon", isHoldingWeapon);
        }

        WeaponVisualGO.SetActive(isHoldingWeapon);
    }
    public void SetWeaponSprite(Sprite weaponSprite)
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = weaponSprite;
    }
}
