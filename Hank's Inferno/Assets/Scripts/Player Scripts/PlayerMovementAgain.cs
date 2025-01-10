using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAgain : MonoBehaviour
{
    // Redigerad från Eskils orginella kod (PlayerMovement.cs) av Dennis

    enum PlayerStates { NORMAL, DASH };

    // Initializing variables
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Animator anim;

    private Rigidbody2D rb;
    private PlayerStates state = PlayerStates.NORMAL;

    [Header("Walking")]
    [SerializeField] private float walkSpeed = 11f;
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float turnAcceleration = 16f;

    private float moveDir = 0;
    private float momentum = 0;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 30f;
    [SerializeField] private float jumpGravity, peakGravity;
    [SerializeField] private float terminalVelocity;
    [SerializeField] private Vector2 groundCheck;

    float gravity = 1;
    bool isGrounded = false;
    bool isJumping = false;
    float jumpBuffer = 0;
    float coyoteTime = 0;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 1f;

    Vector2 dashDir;
    float dashTimer;

    private float h_speed;
    private float v_speed;
    private float x_scale = 0f;
    private float y_scale = 0f;
    private float dash_charge = 0;

    // Input
    float horiInput, vertInput;
    bool jumpInput, jumpInputDown;
    bool dashInputDown;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gravity = rb.gravityScale;
    }

    void Update()
    {
        #region Input
        horiInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        jumpInput = Input.GetButton("Jump");
        if (Input.GetButtonDown("Jump"))
        {
            jumpInputDown = true;
            jumpBuffer = 0.2f;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
            dashInputDown = true;
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Check ground
        isGrounded = Physics2D.BoxCast(transform.position, groundCheck, 0, Vector2.down, 0, whatIsGround);
        if (isGrounded)
            coyoteTime = 0.2f;

        #region States
        switch (state)
        {
            case (PlayerStates.NORMAL):

                // Momentum
                if (momentum != 0f)
                {
                    float mom_spd = 100f;
                    if (!isGrounded)
                        mom_spd = 30f;

                    momentum += Mathf.Clamp(-momentum, -mom_spd * Time.fixedDeltaTime, mom_spd * Time.fixedDeltaTime);
                    //if (horiInput == -Mathf.Sign(momentum))
                        //momentum = Mathf.Max(Mathf.Abs(momentum) - 1, 0) * Mathf.Sign(momentum);
                }

                // Walk
                float acc = acceleration;
                if (Mathf.Abs(horiInput - moveDir) > 1f)
                    acc = turnAcceleration;
                moveDir += Mathf.Min(Mathf.Abs(horiInput - moveDir), acc * Time.fixedDeltaTime) * Mathf.Sign(horiInput - moveDir); // Interpolates towards the desired direction without overshooting

                // Apply horizontal velocity
                float hsp = moveDir * walkSpeed;
                if(momentum != 0f && hsp != 0f && Mathf.Sign(hsp) == -Mathf.Sign(momentum))
                {
                    float chunk = Mathf.Min(Mathf.Abs(hsp), Mathf.Abs(momentum)) * Mathf.Sign(momentum);
                    hsp -= chunk;
                    momentum -= chunk;
                }
                rb.velocity = new Vector2(hsp + momentum, rb.velocity.y);

                // Jump
                if (jumpBuffer > 0f && coyoteTime > 0f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                    isGrounded = false;
                    isJumping = true;

                    jumpBuffer = 0f;
                    coyoteTime = 0f;
                }
                if(isJumping)
                {
                    if (rb.velocity.y <= 0)
                        isJumping = false;
                    else if(!jumpInput)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
                        isJumping = false;
                    }
                }

                // Gravity
                if (isJumping)
                {
                    if(rb.velocity.y < 1f)
                        rb.gravityScale = peakGravity;
                    else
                        rb.gravityScale = jumpGravity;
                }
                else
                    rb.gravityScale = gravity;

                // Dash
                if (dashInputDown)
                {
                    dashTimer = dashDuration;
                    dashDir = new Vector2(horiInput, new Vector2(horiInput, vertInput).normalized.y / 2f);
                    if (Mathf.Sign(momentum) != horiInput)
                        momentum = 0;

                    state = PlayerStates.DASH;
                }

                break;

            case (PlayerStates.DASH):

                rb.velocity = dashDir * dashSpeed + Vector2.right * momentum;

                isJumping = false;
                rb.gravityScale = 0.5f;

                dashTimer -= Time.fixedDeltaTime;
                if (dashTimer <= 0f)
                {
                    state = PlayerStates.NORMAL;
                    if (rb.velocity.y > 0f)
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
                }
                else if (jumpBuffer > 0f && coyoteTime > 0f) // Dash jump
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    momentum = rb.velocity.x;

                    isGrounded = false;
                    isJumping = true;

                    jumpBuffer = 0f;
                    coyoteTime = 0f;
                    dashTimer = 0f;

                    state = PlayerStates.NORMAL;
                }

                break;

        }
        #endregion

        if(coyoteTime > 0f)
            coyoteTime -= Time.fixedDeltaTime;
        if (jumpBuffer > 0f)
            jumpBuffer -= Time.fixedDeltaTime;

        jumpInputDown = false;
        dashInputDown = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draw ground check square
        Gizmos.DrawWireCube(transform.position, new Vector3(groundCheck.x, groundCheck.y, 0.01f));
    }
}
