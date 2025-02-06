using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAgain : MonoBehaviour
{
    // Redigerad från Eskils orginella kod (PlayerMovement.cs) av Dennis

    enum PlayerStates { NORMAL, DASH };

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

        // Få värdet från knapptryck (detta gör det enklare att byta knappar senare, då ligger allt på samma ställe).
        horiInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        jumpInput = Input.GetButton("Jump");

        // Eftersom att alla rörelser händer i FixedUpdate så kan GetButtonDown inte bara vara aktiv en frame, förutom vara aktiv tills nästa gång FixedUpdate körs.
        if (Input.GetButtonDown("Jump"))
        {
            jumpInputDown = true;
            jumpBuffer = 0.2f; // Om man försöker hoppa innan man träffar marken så kommer spelet ihåg det.
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
            dashInputDown = true;
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // En BoxCast kollar om man är på marken eller inte.
        isGrounded = Physics2D.BoxCast(transform.position, groundCheck, 0, Vector2.down, 0, whatIsGround);
        if (isGrounded)
            coyoteTime = 0.2f; // Om en spelare försöker hoppa av en kant och råkar gå av kanten lite för tidigt så låter spelet en fortfarande hoppa.

        #region States
        switch (state)
        {
            case (PlayerStates.NORMAL):

                // Extra fart
                if (momentum != 0f)
                {
                    float mom_spd = 100f;
                    if (!isGrounded)
                        mom_spd = 30f;

                    // En grej som alltid rör sig mot 0, även om värdet är negativt.
                    momentum += Mathf.Clamp(-momentum, -mom_spd * Time.fixedDeltaTime, mom_spd * Time.fixedDeltaTime);
                }

                // Gående
                float acc = acceleration;
                if (Mathf.Abs(horiInput - moveDir) > 1f)
                    acc = turnAcceleration;
                moveDir += Mathf.Min(Mathf.Abs(horiInput - moveDir), acc * Time.fixedDeltaTime) * Mathf.Sign(horiInput - moveDir); // Går mot det ideala värdet utan att gå över eller under.

                // Lägg till den bestämda farten
                float hsp = moveDir * walkSpeed;
                if(momentum != 0f && hsp != 0f && Mathf.Sign(hsp) == -Mathf.Sign(momentum))
                {
                    // Om spelaren försöker gå emot deras "momentum" så tas den farten bort snabbare, men de tappar den farten i sitt gående (vilket bara är plus minus noll).
                    float chunk = Mathf.Min(Mathf.Abs(hsp), Mathf.Abs(momentum)) * Mathf.Sign(momentum);
                    hsp -= chunk;
                    momentum -= chunk;
                }

                // Tillslut så läggs den farten till som den horizontella hastigheten.
                rb.velocity = new Vector2(hsp + momentum, rb.velocity.y);

                // Hoppande
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
                    // Om spelarens hastighet är negativ eller om de inte håller ner hopp knappen så slutar de åka upp.
                    if (rb.velocity.y <= 0)
                        isJumping = false;
                    else if(!jumpInput)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f); // Detta gör så att spelaren kan göra ett kortare hopp om de vill sluta hoppa (som i Mario spelen).
                        isJumping = false;
                    }
                }

                // Gravitation
                if (isJumping)
                {
                    // Om spelaren hoppar så ändras deras gravitation så att de har mer kontroll, speciellt när de når toppen av deras hopp.
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

                    // Om man går emot sin fart så nollställs den helt.
                    if (Mathf.Sign(momentum) != horiInput)
                        momentum = 0;

                    state = PlayerStates.DASH;
                }

                break;

            case (PlayerStates.DASH):

                // Spelaren färdas åt det hållet de valde när de påbörjade sin dash.
                rb.velocity = dashDir * dashSpeed + Vector2.right * momentum;

                isJumping = false;
                rb.gravityScale = 0.5f; // Gravitationen är mycket lägre så att man åker rakare, men inte helt borta heller.

                dashTimer -= Time.fixedDeltaTime;
                if (dashTimer <= 0f)
                {
                    // När tiden för dashen tar slut så återgår man till sitt normala läge.
                    state = PlayerStates.NORMAL;
                    if (rb.velocity.y > 0f)
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
                }
                else if (jumpBuffer > 0f && coyoteTime > 0f)
                {
                    // Om man nuddar marken och hoppar så gör man en "dash jump", vilket låter dig behålla din fart.
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

        // Spel förbättrare som räknas ner.
        if(coyoteTime > 0f)
            coyoteTime -= Time.fixedDeltaTime;
        if (jumpBuffer > 0f)
            jumpBuffer -= Time.fixedDeltaTime;

        // Alla knapp relaterade variablar som bara får vara på en gång stängs av på slutet av allt.
        jumpInputDown = false;
        dashInputDown = false;
    }

    void OnDrawGizmosSelected()
    {
        // Visuellt visar vart koden tittar efter marken.
        Gizmos.DrawWireCube(transform.position, new Vector3(groundCheck.x, groundCheck.y, 0.01f));
    }
}
