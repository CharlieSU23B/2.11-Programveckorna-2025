using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Initializing variables
    public Rigidbody2D rb;
    private float h_speed;
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
    string state = "FREE";
    private float dash_charge = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case ("FREE"):
                {
                    // Walking code
                    float walk_dir = Input.GetAxisRaw("Horizontal");

                    if (walk_dir != 0)
                    {
                        h_speed += ((max_speed * walk_dir) - h_speed) * 10f * Time.deltaTime;
                    }
                    else
                    {
                        h_speed += (0 - h_speed) * 18f * Time.deltaTime;
                    }

                    // Ground check
                    if (Physics2D.BoxCast(transform.position, box_mask, 0, -transform.up, box_distance, ground_layer) && rb.velocity.y <= 0f)
                    {
                        if (grounded == false)
                        {
                            if (GameObject.Find("Main Camera"))
                            {
                                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 0.625f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 0.625f;
                            }

                            x_scale = 1.5f;
                            y_scale = 0.5f;
                        }

                        grounded = true;
                        v_speed = 0;
                    }
                    else
                    {
                        grounded = false;
                        v_speed -= 50f * Time.deltaTime;
                    }

                    // Jump
                    if (!Input.GetKey(KeyCode.Space))
                    {
                        if (v_speed > 0 && space_down == true)
                        {
                            v_speed /= 3;
                        }

                        space_down = false;
                    }

                    if (grounded && Input.GetKey(KeyCode.Space) && space_down == false)
                    {
                        v_speed = 25;

                        x_scale = 0.5f;
                        y_scale = 1.5f;

                        space_down = true;

                        grounded = false;
                    }

                    // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale, y_scale);

                    // Dash init
                    if(Input.GetKey(KeyCode.LeftShift))
                    {
                        h_speed = 0;
                        v_speed = 0;

                        x_scale = 1;
                        y_scale = 1;

                        dash_charge = 1;

                        state = "DASH CHARGE";
                    }
                }
                break;

            case ("DASH CHARGE"):
                {
                    // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale -= 0.025f;
                    y_scale -= 0.025f;

                    dash_charge += 0.025f;

                    fake_sprite.transform.localScale = new Vector2(x_scale, y_scale);

                    if (!Input.GetKey(KeyCode.LeftShift) || x_scale <= 0f)
                    {
                        h_speed = Input.GetAxisRaw("Horizontal");
                        v_speed = Input.GetAxisRaw("Vertical");

                        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 2f * dash_charge) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 2f*dash_charge;

                        state = "DASH";
                    }
                }
                break;

            case ("DASH"):
                {
                    // Rigidbody
                    rb.velocity = new Vector2(h_speed,v_speed).normalized * 24f * dash_charge;

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale, y_scale);

                    dash_charge -= 0.1f;

                    if (dash_charge <= 0f) state = "FREE";
                }
                break;
        }

        Debug.Log(rb.velocity);
    }
}
