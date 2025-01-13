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
    private bool shift_down = false;
    public LayerMask ground_layer;
    public float box_distance = 0;
    public Vector2 box_mask;
    private float v_speed;
    private float x_scale = 0f;
    private float y_scale = 0f;
    public GameObject fake_sprite;
    public string state = "FREE";
    private float dash_charge = 0;
    private float jump_buffer = 0;
    private bool dash = false;
    public GameObject elevator;
    public float elevator_timer = 0;
    private float elevator_y_speed = 0;
    public GameObject dust;
    public float rooms = 10;
    public bool[] room_entered = new bool[10];
    public int room_i = 0;
    private float flip_scale = 1;
    private float rooms_count = 0;
    // Start is called before the first frame update
    void Start()
    {
        float _choose = Random.Range(0, 100);
        int _current_i = 0;

        for (room_i = 0; room_i < rooms; room_i++)
        {
            if (room_i * (100 / rooms) <= _choose)
            {
                transform.position = new Vector3(0, -room_i * 32, 0);

                GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y = -room_i * 32;
                GameObject.Find("Door").GetComponent<DoorCode>().transform.position = new Vector3(0, transform.position.y - 0.5f, 0);

                _current_i = room_i;
            }
        }

        room_i = _current_i;
        rooms_count++;
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

                        flip_scale = walk_dir;
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

                            Instantiate(dust, transform.position + new Vector3(0,1f,0), Quaternion.identity);

                            x_scale = 1.5f;
                            y_scale = 0.5f;
                        }

                        dash = true;

                        jump_buffer = 1;

                        grounded = true;
                        v_speed = 0;
                    }
                    else
                    {
                        grounded = false;
                        v_speed -= 50f * Time.deltaTime;

                        jump_buffer -= 10f * Time.deltaTime;
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

                    if (jump_buffer > 0 && Input.GetKey(KeyCode.Space) && space_down == false)
                    {
                        v_speed = 25;

                        x_scale = 0.5f;
                        y_scale = 1.5f;

                        space_down = true;

                        jump_buffer = 0;
                    }

                    // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale*flip_scale* 4.5f, y_scale * 4.5f);

                    // Dash init
                    if(!Input.GetKey(KeyCode.LeftShift))
                    {
                        shift_down = false;
                    }

                    if(Input.GetKey(KeyCode.LeftShift) && dash == true && shift_down == false)
                    {
                        h_speed = 0;
                        v_speed = 0;

                        x_scale = 1;
                        y_scale = 1;

                        dash_charge = 1;

                        dash = false;

                        shift_down = true;

                        state = "DASH CHARGE";
                    }
                }
                break;

            case ("DASH CHARGE"):
                {
                    // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale -= 0.2f;
                    y_scale -= 0.2f;

                    dash_charge += 0.2f;

                    fake_sprite.transform.localScale = new Vector2(x_scale * flip_scale * 4.5f, y_scale * 4.5f);

                    if (x_scale <= 0f)
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
                    rb.velocity = new Vector2(h_speed,v_speed).normalized * 28f * dash_charge;

                    // Land
                    if (Physics2D.BoxCast(transform.position, box_mask, 0, -transform.up, box_distance, ground_layer) && rb.velocity.y < 0f)
                    {              
                        v_speed = 1f;
                        h_speed = Input.GetAxisRaw("Horizontal")*15;
                        x_scale = 0.5f;
                        y_scale = 1.5f;
                        dash_charge = 2.25f;
                        dash = true;
                        Instantiate(dust, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);

                        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 2f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 2f;
                    }

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale * flip_scale * 4.5f, y_scale * 4.5f);

                    dash_charge -= 0.1f;

                    if (dash_charge <= 0f) state = "FREE";
                }
                break;

            case ("ELEVATOR"):
                {
                    rb.velocity = new Vector2(0, 0);
                    h_speed = 0;
                    v_speed = 0;
                    
                    if (elevator_timer > 0)
                    {
                        transform.position += ((elevator.transform.position + new Vector3(0, -2f, 0)) - transform.position) * 10f * Time.deltaTime;
                    }
                    else
                    {
                        elevator.transform.position += new Vector3(0, -elevator_y_speed, 0);
                        elevator_y_speed += 0.1f;

                        if(elevator_y_speed > 5f)
                        {
                            GameObject.Find("Main Camera").GetComponent<CameraController>().fade = true;

                            if(GameObject.Find("Main Camera").GetComponent<CameraController>().alpha >= 1)
                            {
                                if (rooms_count < 10)
                                {
                                    state = "ELEVATOR OPEN";
                                    GameObject.Find("Main Camera").GetComponent<CameraController>().fade = false;
                                    transform.position = elevator.transform.position;

                                    room_entered[room_i] = true;

                                    while (room_entered[room_i] == true)
                                    {
                                        float _choose = Random.Range(0, 100);
                                        int _current_i = 0;

                                        room_i = 0;
                                        for (room_i = 0; room_i < rooms; room_i++)
                                        {
                                            if (room_i * (100 / rooms) <= _choose)
                                            {
                                                transform.position = new Vector3(0, -room_i * 32, 0);

                                                GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y = -room_i * 32;
                                                _current_i = room_i;
                                            }
                                        }
                                        room_i = _current_i;

                                        rooms_count++;
                                    }

                                    elevator.transform.position = new Vector3(0, GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y + 32, 0);

                                    GameObject.Find("Main Camera").transform.position = new Vector3(0, GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y, -10);

                                    elevator_timer = 18f;
                                    elevator_y_speed = 0;
                                }
                                else
                                {
                                    state = "ELEVATOR OPEN";
                                    GameObject.Find("Main Camera").GetComponent<CameraController>().fade = false;
                                    transform.position = elevator.transform.position;

                                    room_entered[room_i] = true;

                                    transform.position = new Vector3(0, -320, 0);

                                    GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y = -320;

                                    elevator.transform.position = new Vector3(0, GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y + 32, 0);

                                    GameObject.Find("Main Camera").transform.position = new Vector3(0, GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y, -10);

                                    elevator_timer = 18f;
                                    elevator_y_speed = 0;
                                }
                            }
                        }

                        transform.position = elevator.transform.position + new Vector3(0, -2f, 0);
                    }

                    elevator_timer -= 10f * Time.deltaTime;
                }
                break;

            case ("ELEVATOR OPEN"):
                {
                    elevator.transform.position += (new Vector3(0, GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y - 2.5f, 0) - transform.position) * 10f * Time.deltaTime;
                    transform.position = elevator.transform.position + new Vector3(0, -2f, 0);
                    elevator_timer -= 10f * Time.deltaTime;

                    if(elevator_timer <= 0f)
                    {
                        state = "FREE";
                    }
                }
                break;
        }
    }
}
