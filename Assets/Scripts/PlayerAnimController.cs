using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void SetIsGrounded(bool isGrounded) => animator.SetBool("grounded", isGrounded);
    public void SetVelocity(float velocity) => animator.SetFloat("velocity", Mathf.Abs(velocity));
}
