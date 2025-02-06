using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementAgain : MonoBehaviour
{
    // Redigerad fr�n Eskils orginella kod (PlayerMovement.cs) av Dennis

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

        // F� v�rdet fr�n knapptryck (detta g�r det enklare att byta knappar senare, d� ligger allt p� samma st�lle).
        horiInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");

        jumpInput = Input.GetButton("Jump");

        // Eftersom att alla r�relser h�nder i FixedUpdate s� kan GetButtonDown inte bara vara aktiv en frame, f�rutom vara aktiv tills n�sta g�ng FixedUpdate k�rs.
        if (Input.GetButtonDown("Jump"))
        {
            jumpInputDown = true;
            jumpBuffer = 0.2f; // Om man f�rs�ker hoppa innan man tr�ffar marken s� kommer spelet ih�g det.
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
            dashInputDown = true;
        #endregion
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // En BoxCast kollar om man �r p� marken eller inte.
        isGrounded = Physics2D.BoxCast(transform.position, groundCheck, 0, Vector2.down, 0, whatIsGround);
        if (isGrounded)
            coyoteTime = 0.2f; // Om en spelare f�rs�ker hoppa av en kant och r�kar g� av kanten lite f�r tidigt s� l�ter spelet en fortfarande hoppa.

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

                    // En grej som alltid r�r sig mot 0, �ven om v�rdet �r negativt.
                    momentum += Mathf.Clamp(-momentum, -mom_spd * Time.fixedDeltaTime, mom_spd * Time.fixedDeltaTime);
                }

                // G�ende
                float acc = acceleration;
                if (Mathf.Abs(horiInput - moveDir) > 1f)
                    acc = turnAcceleration;
                moveDir += Mathf.Min(Mathf.Abs(horiInput - moveDir), acc * Time.fixedDeltaTime) * Mathf.Sign(horiInput - moveDir); // G�r mot det ideala v�rdet utan att g� �ver eller under.

                // L�gg till den best�mda farten
                float hsp = moveDir * walkSpeed;
                if(momentum != 0f && hsp != 0f && Mathf.Sign(hsp) == -Mathf.Sign(momentum))
                {
                    // Om spelaren f�rs�ker g� emot deras "momentum" s� tas den farten bort snabbare, men de tappar den farten i sitt g�ende (vilket bara �r plus minus noll).
                    float chunk = Mathf.Min(Mathf.Abs(hsp), Mathf.Abs(momentum)) * Mathf.Sign(momentum);
                    hsp -= chunk;
                    momentum -= chunk;
                }

                // Tillslut s� l�ggs den farten till som den horizontella hastigheten.
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
                    // Om spelarens hastighet �r negativ eller om de inte h�ller ner hopp knappen s� slutar de �ka upp.
                    if (rb.velocity.y <= 0)
                        isJumping = false;
                    else if(!jumpInput)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f); // Detta g�r s� att spelaren kan g�ra ett kortare hopp om de vill sluta hoppa (som i Mario spelen).
                        isJumping = false;
                    }
                }

                // Gravitation
                if (isJumping)
                {
                    // Om spelaren hoppar s� �ndras deras gravitation s� att de har mer kontroll, speciellt n�r de n�r toppen av deras hopp.
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

                    // Om man g�r emot sin fart s� nollst�lls den helt.
                    if (Mathf.Sign(momentum) != horiInput)
                        momentum = 0;

                    state = PlayerStates.DASH;
                }

                break;

            case (PlayerStates.DASH):

                // Spelaren f�rdas �t det h�llet de valde n�r de p�b�rjade sin dash.
                rb.velocity = dashDir * dashSpeed + Vector2.right * momentum;

                isJumping = false;
                rb.gravityScale = 0.5f; // Gravitationen �r mycket l�gre s� att man �ker rakare, men inte helt borta heller.

                dashTimer -= Time.fixedDeltaTime;
                if (dashTimer <= 0f)
                {
                    // N�r tiden f�r dashen tar slut s� �terg�r man till sitt normala l�ge.
                    state = PlayerStates.NORMAL;
                    if (rb.velocity.y > 0f)
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
                }
                else if (jumpBuffer > 0f && coyoteTime > 0f)
                {
                    // Om man nuddar marken och hoppar s� g�r man en "dash jump", vilket l�ter dig beh�lla din fart.
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

        // Spel f�rb�ttrare som r�knas ner.
        if(coyoteTime > 0f)
            coyoteTime -= Time.fixedDeltaTime;
        if (jumpBuffer > 0f)
            jumpBuffer -= Time.fixedDeltaTime;

        // Alla knapp relaterade variablar som bara f�r vara p� en g�ng st�ngs av p� slutet av allt.
        jumpInputDown = false;
        dashInputDown = false;
    }

    void OnDrawGizmosSelected()
    {
        // Visuellt visar vart koden tittar efter marken.
        Gizmos.DrawWireCube(transform.position, new Vector3(groundCheck.x, groundCheck.y, 0.01f));
    }
}
