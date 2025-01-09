using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Initializing variables
    public Rigidbody2D rb;
    private float walk_speed;
    private float max_speed = 8.5f;
    private bool grounded = false;
    private bool space_down = false;
    public LayerMask ground_layer;
    public float box_distance = 0;
    public Vector2 box_mask;
    private float v_speed;
    private float x_scale = 0f;
    private float y_scale = 0f;
    public GameObject fake_sprite;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Walking code
        float walk_dir = Input.GetAxisRaw("Horizontal");

        if(walk_dir != 0)
        {
            walk_speed += ((max_speed * walk_dir) - walk_speed) * 10f * Time.deltaTime;
        }
        else
        {
            walk_speed += (0 - walk_speed) * 18f * Time.deltaTime;
        }

        // Ground check
        if(Physics2D.BoxCast(transform.position, box_mask, 0, -transform.up, box_distance, ground_layer) && rb.velocity.y <= 0f)
        {
            if (grounded == false)
            {
                x_scale = 1.5f;
                y_scale = 0.5f;
            }

            grounded = true;
            v_speed = 0;
        }
        else
        {
            grounded = false;
            v_speed -= 3f * Time.deltaTime;
        }

        // Jump
        if (!Input.GetKey(KeyCode.Space))
        {
            if(rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
            }

            space_down = false;
        }

        if (grounded && Input.GetKey(KeyCode.Space) && space_down == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, 12);

            x_scale = 0.5f;
            y_scale = 1.5f;

            space_down = true;

            grounded = false;
        }

        // Rigidbody
        rb.velocity = new Vector2(walk_speed, rb.velocity.y+v_speed);

        // Squash and Stretch
        x_scale += (1 - x_scale) * 10f * Time.deltaTime;
        y_scale += (1 - y_scale) * 10f * Time.deltaTime;

        fake_sprite.transform.localScale = new Vector2(x_scale, y_scale);
    }
}
