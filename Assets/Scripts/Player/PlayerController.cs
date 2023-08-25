using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : Entity
{
    public bool isOld;
    public Vector3 forwardVector { get; private set; }
    public bool isFacingLeft { get; private set; }
    public bool isGrounded { get; private set; }

    [Header("Values")]
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

    Vector2 rbVelocityHash = Vector2.zero;

    int hihihiha = 0;

    public override void Start()
    {
        _health = _maxHealth;

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
    }

    public void SpawnPlayer(Vector2 position)
    {
        transform.position = position;

        overrideVelocity = true;
        overrideDisableTime = Time.time + .2f;
    }
    public override void Die()
    {
        GameManager.Instance.EndGame();
        base.Die();
    }

    bool isHoldingHorizontalInput;
    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isHoldingHorizontalInput = Input.GetButton("Horizontal");

        if (overrideVelocity == true)
        {
            if(horizontalInput != 0)
            { overrideVelocity = false; }

            if(isHoldingHorizontalInput == false && isGrounded && overrideDisableTime < Time.time)
            { overrideVelocity = false; }
        }

        jumpPressedDown = Input.GetButtonDown("Jump");
        jumpPressedUp = Input.GetButtonUp("Jump");

        if (jumpPressedDown) targetTime_CoyoteJump = Time.time + coyoteJumpWindow;
        else if (jumpPressedUp) targetTime_CoyoteJump = -1;
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
            float airBonus = isGrounded ? 1 : airSpeedModifier;
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

        if (targetTime_CoyoteJump > Time.time && isGrounded)
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
            isGrounded = true;
            targetTime_CoyoteTime = Time.time + coyoteTimeWindow;

            overrideVelocity = false;
        }
        else if (targetTime_CoyoteTime >= Time.time)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        UE_Grounded.Invoke(isGrounded);
    }
    private void IntensifyGravityScale()
    {
        if (rbVelocityHash.y < 0)
            rb.gravityScale = gravityScale + fallingAccelerationIntensity;
        else
            rb.gravityScale = gravityScale;
    }
    void CheckFlipNeed()
    {
        if ((horizontalInput > 0 && isFacingLeft) || (horizontalInput < 0 && !isFacingLeft))
            Flip();
    }
    void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        isFacingLeft = !isFacingLeft;
        forwardVector = isFacingLeft ? new Vector2(-1, 0) : new Vector2(1, 0);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(GroundCheck.position, groundCheckRadius);
    }

    internal override void UpdateBehaviour(){}//Bok gib kod HIHIHIHA
}