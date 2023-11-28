using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Entity
{
    public Vector3 ForwardVector { get; private set; }
    public bool IsParrying { get; private set; }
    public bool IsFacingLeft { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool IsOld { get; private set; }

    [Header("Values")]
    [SerializeField] float parryDuration = .3f;
    [SerializeField] float parryCooldown = .9f;
    [Tooltip("How long will fully accelerating take")]
    [SerializeField, Min(0.001f)] float  accelerationTime = .1f;
    [Tooltip("How long will fully decelerating take")]
    [SerializeField, Min(0.001f)] float  decelerationTime = .2f;
    [SerializeField, Range(0, 2)] float airSpeedModifier = .8f;
    [SerializeField] float maxSpeed = 25f;
    [SerializeField] float jumpForce = 35f;
    [SerializeField] float fallingAccelerationIntensity = 8f;
    [SerializeField] float gravityScale = 10;
    [SerializeField] float coyoteTimeWindow = 0.1f;
    [SerializeField] float coyoteJumpWindow = 0.05f;
    [SerializeField] Vector2 groundCheckRadius = Vector2.one * .2f;
    [SerializeField, Min(0)] float jumpCancelPower = 1.5f;

    [Header("Reference")]
    [SerializeField] LayerMask GroundLayers;
    [SerializeField] GameObject Barrel;
    [SerializeField] GameObject Visuals;
    [SerializeField] Transform GroundCheck;
    [SerializeField] PlayerWeaponController playerWeaponController;

    [Header("Animation Events")]
    [SerializeField] UnityEvent<bool> UE_Grounded;
    [SerializeField] UnityEvent<float> UE_HorizontalVelocity;

    float horizontalInput;
    bool jumpPressedDown;
    bool jumpPressedUp;

    bool overrideVelocity;
    float overrideDisableTime = float.MaxValue;

    float targetTime_CoyoteJump = -1;
    float targetTime_CoyoteTime = -1;
    float targetTime_Parry = -1;

    Vector2 rbVelocityHash = Vector2.zero;

    public override void Start()
    {
        SetHealth(b_maxHealth);

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale;

        SetUpdateCoroutine(false);
    }
    void Update()
    {
        GetInput();
        ApplyMovement();
        CheckGround();
        IntensifyGravityScale();
        CheckValues();
    }

    public void PlayerIsSelected()
    {
        IsOld = true;
    }
    public void SpawnPlayer(Vector2 position)
    {
        transform.position = position;

        overrideVelocity = true;
        overrideDisableTime = Time.time + .2f;
    }
    public void ParrySuccessful()
    {
        IsParrying = false;
    }

    bool isHoldingHorizontalInput;
    void GetInput()
    {
        //if(b_inputEnabled)
        //{
            horizontalInput = Input.GetAxisRaw("Horizontal");
            isHoldingHorizontalInput = Input.GetButton("Horizontal");

            if (overrideVelocity == true)
            {
                if (horizontalInput != 0)
                { overrideVelocity = false; }

                if (isHoldingHorizontalInput == false && IsGrounded && overrideDisableTime < Time.time)
                { overrideVelocity = false; }
            }

            jumpPressedDown = Input.GetButtonDown("Jump");
            jumpPressedUp = Input.GetButtonUp("Jump");

            if (jumpPressedDown) targetTime_CoyoteJump = Time.time + coyoteJumpWindow;
            else if (jumpPressedUp) targetTime_CoyoteJump = -1;

        if (IsParrying == false && targetTime_Parry <= Time.time && Input.GetButtonDown("Parry"))
        {
            IsParrying = true;
            targetTime_Parry = Time.time + parryDuration;
        }
        //}
        //else
        //{
        //    horizontalInput = 0;
        //    isHoldingHorizontalInput = false;
        //    jumpPressedDown = false;
        //    jumpPressedUp = false;
        //}
    }

    void ApplyMovement()
    {
        Vector2 targetVelocity = Vector2.zero;
        rbVelocityHash = rb.velocity;
        float hMovementThisFrame = 0;

        targetVelocity.y = rbVelocityHash.y;

        if (overrideVelocity)
        {
            targetVelocity.x = rb.velocity.x;
        }
        else if (Mathf.Abs(horizontalInput) > 0)//accelerate
        {
            float airBonus = IsGrounded ? 1 : airSpeedModifier;
            hMovementThisFrame = maxSpeed / accelerationTime * airBonus * Time.deltaTime * horizontalInput;

            if (hMovementThisFrame > 0)
            {
                targetVelocity.x = Mathf.Min(rbVelocityHash.x + hMovementThisFrame, maxSpeed);
            }
            else if (hMovementThisFrame < 0)
            {
                targetVelocity.x = Mathf.Max(rbVelocityHash.x + hMovementThisFrame, -maxSpeed);
            }
            else
            {
                targetVelocity.x = rbVelocityHash.x;
            }
        }
        else if (rbVelocityHash.x != 0)//decelerate (if not already stopping)
        {
            hMovementThisFrame = maxSpeed / decelerationTime * Time.deltaTime;
            float rbVelocitySign = Mathf.Sign(rbVelocityHash.x);

            targetVelocity.x = rbVelocityHash.x + hMovementThisFrame * -rbVelocitySign;

            if (rbVelocitySign > 0)
            {
                targetVelocity.x = Mathf.Max(targetVelocity.x, 0);
            }
            else if (rbVelocitySign < 0)
            {
                targetVelocity.x = Mathf.Min(targetVelocity.x, 0);
            }
        }

        CheckFlipNeed();

        if (targetTime_CoyoteJump > Time.time && IsGrounded)
        {
            targetVelocity.y = jumpForce;
            targetTime_CoyoteJump = -1;
        }
        else if (rbVelocityHash.y > 0 && jumpPressedUp)
        {
            targetVelocity.y = rbVelocityHash.y / (1 + jumpCancelPower);
        }

        UE_HorizontalVelocity?.Invoke(targetVelocity.x);
        rb.velocity = targetVelocity;
    }

    void CheckGround()
    {
        if (Physics2D.OverlapBox(GroundCheck.position, groundCheckRadius, 0, GroundLayers))
        {
            IsGrounded = true;
            targetTime_CoyoteTime = Time.time + coyoteTimeWindow;

            overrideVelocity = false;
        }
        else if (targetTime_CoyoteTime >= Time.time)
        {
            IsGrounded = true;
        }
        else
        {
            IsGrounded = false;
        }

        UE_Grounded.Invoke(IsGrounded);
    }
    void IntensifyGravityScale()
    {
        if (rbVelocityHash.y < 0)
            rb.gravityScale = gravityScale + fallingAccelerationIntensity;
        else
            rb.gravityScale = gravityScale;
    }
    void CheckValues()
    {
        if(IsParrying && targetTime_Parry <= Time.time)
        {
            IsParrying = false;
            targetTime_Parry = Time.time + parryCooldown;
        }
    }
    void CheckFlipNeed()
    {
        if ((horizontalInput > 0 && IsFacingLeft) || (horizontalInput < 0 && !IsFacingLeft))
            Flip();
    }
    void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        IsFacingLeft = !IsFacingLeft;
        ForwardVector = IsFacingLeft ? new Vector2(-1, 0) : new Vector2(1, 0);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GroundCheck.position, groundCheckRadius);
    }

    internal override void UpdateBehaviour(){}//Bok gib kod HIHIHIHA
}