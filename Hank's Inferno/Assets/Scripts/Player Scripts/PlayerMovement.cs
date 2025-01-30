using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PlayerMovement : MonoBehaviour
{
    // Initializing variables
    public Rigidbody2D rb;
    public float h_speed;
    private float max_speed = 8.5f;
    private bool grounded = false;
    private bool space_down = false;
    private bool shift_down = false;
    public LayerMask ground_layer;
    public float box_distance = 0;
    public Vector2 box_mask;
    public float v_speed;
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
    public float flip_scale = 1;
    private float rooms_count = 0;
    public int enemies_to_kill = 0;
    private float iframes = 0;
    public GameObject explosion;
    public SpriteRenderer flash_1;
    public SpriteRenderer flash_2;
    public float flash = 0f;
    private bool fall = false;
    public float healing = 1; // healing variable is now withdrawals
    public float healing_draw = 0;
    public AudioSource jump_sound;
    public AudioSource land_sound;
    public AudioSource step_sound;
    public AudioSource dash_sound;
    public AudioSource hurt_sound;
    public AudioSource death_sound;
    public AudioSource elevator_sound;
    public AudioSource heal_sound;
    public AudioSource music;

    // Start is called before the first frame update
    void Start()
    {
        music.Stop();
        music.Play();

        healing = 1;

        elevator = GameObject.Find("Door");

        if (elevator != null)
        {
            GameObject.Find("Main Camera").GetComponent<CameraController>().alpha = 1;
            state = "ELEVATOR OPEN";
            GameObject.Find("Main Camera").GetComponent<CameraController>().fade = false;
            transform.position = elevator.transform.position;

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

            elevator.transform.position = new Vector3(0, GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y + 32, 0);

            GameObject.Find("Main Camera").transform.position = new Vector3(0, GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y, -10);

            elevator_timer = 18f;
            elevator_y_speed = 0;
        }
        else
            state = "FREE";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(healing <= 0f)
        {
            max_speed = 2.5f;
        }
        else
        {
            max_speed = 8.5f;
        }

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

                            land_sound.Play();
                        }

                        dash = true;

                        jump_buffer = 1;

                        grounded = true;
                        v_speed = 0;
                    }
                    else
                    {
                        grounded = false;
                        v_speed -= 70f * Time.deltaTime;

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
                        fake_sprite.GetComponent<Animator>().Play("PlayerJump");

                        fall = false;

                        v_speed = 25;

                        x_scale = 0.5f;
                        y_scale = 1.5f;

                        space_down = true;

                        jump_buffer = 0;

                        jump_sound.Play();
                    }

                    if(v_speed <= 0)
                    {
                        if (fall == false)
                        {
                            fake_sprite.GetComponent<Animator>().Play("PlayerFall");

                            x_scale = 0.75f;
                            y_scale = 1.25f;

                            fall = true;
                        }
                    }

                    if(grounded)
                    {
                        if(walk_dir != 0)
                        {
                            fake_sprite.GetComponent<Animator>().Play("PlayerRun");

                            if(fake_sprite.GetComponent<SpriteRenderer>().sprite.name == "Hank_run-Sheet_1"
                                || fake_sprite.GetComponent<SpriteRenderer>().sprite.name == "Hank_run-Sheet_4")
                            {
                                step_sound.Play();
                            }
                        }
                        else
                        {
                            fake_sprite.GetComponent<Animator>().Play("PlayerIdle");
                        }
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
                    fake_sprite.GetComponent<Animator>().Play("PlayerIdle", 0, 0);

                    // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale -= 0.35f;
                    y_scale -= 0.35f;

                    dash_charge += 0.35f;

                    fake_sprite.transform.localScale = new Vector2(x_scale * flip_scale * 4.5f, y_scale * 4.5f);

                    if (x_scale <= 0f)
                    {
                        h_speed = Input.GetAxisRaw("Horizontal");
                        v_speed = Input.GetAxisRaw("Vertical");

                        dash_sound.Play();

                        if(healing > 0) iframes = 2.25f;

                        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 2f * dash_charge) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 2f*dash_charge;

                        state = "DASH";
                    }
                }
                break;

            case ("DASH"):
                {
                    fake_sprite.GetComponent<Animator>().Play("PlayerIdle", 0, 0);

                    // Rigidbody
                    if (healing <= 0)
                    {
                        rb.velocity = new Vector2(h_speed, v_speed * 2f).normalized * 22f * dash_charge;
                    }
                    else
                    {
                        rb.velocity = new Vector2(h_speed, v_speed * 2f).normalized * 41 * dash_charge;
                    }

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

                        dash_sound.Play();

                        iframes = 1.5f;

                        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 2f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 2f;
                    }

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale * flip_scale * 4.5f, y_scale * 4.5f);

                    dash_charge -= 0.165f;

                    if (dash_charge <= 0f) state = "FREE";
                }
                break;

            case ("ELEVATOR"):
                {
                    fake_sprite.GetComponent<Animator>().Play("PlayerIdle", 0, 0);

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

                                    room_i = 10;

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
                    fake_sprite.GetComponent<Animator>().Play("PlayerIdle", 0, 0);

                    elevator.transform.position += (elevator.GetComponent<DoorCode>().door_pos[room_i] - transform.position) * 10f * Time.deltaTime;
                    transform.position = elevator.transform.position + new Vector3(0, -2f, 0);
                    elevator_timer -= 10f * Time.deltaTime;

                    if(elevator_timer <= 0f)
                    {
                        heal_sound.Play();

                        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 8f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 8f;
                        if (GameObject.Find("PlayerHealth") != null) GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().hp++;
                        if (GameObject.Find("PlayerHealth") != null) GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().scale[GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().hp - 1] = 0;

                        state = "FREE";
                    }
                }
                break;
        }
        
        if(healing <= 0)
        {
            GameObject.Find("Withdrawals").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("Withdrawals (1)").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("Withdrawals (2)").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("Withdrawals (3)").GetComponent<SpriteRenderer>().enabled = true;
            if(GameObject.Find("Main Camera").GetComponent<CameraController>().flash_alpha <= 0) GameObject.Find("Main Camera").GetComponent<CameraController>().flash_alpha = 0.2f;
        }
        else
        {
            GameObject.Find("Withdrawals").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("Withdrawals (1)").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("Withdrawals (2)").GetComponent<SpriteRenderer>().enabled = false;
            GameObject.Find("Withdrawals (3)").GetComponent<SpriteRenderer>().enabled = false;
        }
        
        healing -= 0.025f * Time.fixedDeltaTime;

        healing_draw += (healing - healing_draw) * 10f * Time.deltaTime;

        healing = Mathf.Clamp(healing, 0, 1);
        healing_draw = Mathf.Clamp(healing_draw, 0, 1);

        flash_1.color = new Color(1, 1, 1, flash);
        flash_2.color = new Color(1, 1, 1, flash);
        flash -= 0.1f;
        flash = Mathf.Clamp(flash, 0, 1);

        flash_1.sprite = fake_sprite.GetComponent<SpriteRenderer>().sprite;
        flash_2.sprite = fake_sprite.GetComponent<SpriteRenderer>().sprite;

        iframes -= 10f * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy")
        {
            if(iframes <= 0f)
            {
                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 8f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 8f;
                if (GameObject.Find("PlayerHealth") != null) GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().scale[GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().hp-1] = 0;
                if (GameObject.Find("PlayerHealth") != null) GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().hp--;

                GameObject _e1 = Instantiate(explosion, transform.position, Quaternion.identity);
                _e1.GetComponent<ExplosionCode>().timer = 0.25f;
                _e1.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);
                _e1.GetComponent<ExplosionCode>().create_times = 0;
                _e1.GetComponent<SpriteRenderer>().sortingOrder = 12;
                _e1.GetComponent<ExplosionCode>().scale = 12f;
                _e1.GetComponent<ExplosionCode>().un_timed = true;

                for (int _i = 0; _i < 3; _i++)
                {
                    GameObject _e = Instantiate(explosion, transform.position, Quaternion.identity);
                    _e.GetComponent<ExplosionCode>().create_times = Random.Range(3, 7);
                    _e.GetComponent<ExplosionCode>().dir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized;
                    _e.GetComponent<ExplosionCode>().timer = Random.Range(0.1f, 0.25f);
                }

                flash = 1;
                death_sound.Play();
                iframes = 6f;
            }
        }

        if (collision.tag == "Spike")
        {
            if (iframes <= 0f)
            {
                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 8f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 8f;
                if (GameObject.Find("PlayerHealth") != null) GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().scale[GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().hp - 1] = 0;
                if (GameObject.Find("PlayerHealth") != null) GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().hp--;

                GameObject _e1 = Instantiate(explosion, transform.position, Quaternion.identity);
                _e1.GetComponent<ExplosionCode>().timer = 0.25f;
                _e1.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);
                _e1.GetComponent<ExplosionCode>().create_times = 0;
                _e1.GetComponent<SpriteRenderer>().sortingOrder = 12;
                _e1.GetComponent<ExplosionCode>().scale = 12f;
                _e1.GetComponent<ExplosionCode>().un_timed = true;

                for (int _i = 0; _i < 3; _i++)
                {
                    GameObject _e = Instantiate(explosion, transform.position, Quaternion.identity);
                    _e.GetComponent<ExplosionCode>().create_times = Random.Range(3, 7);
                    _e.GetComponent<ExplosionCode>().dir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized;
                    _e.GetComponent<ExplosionCode>().timer = Random.Range(0.1f, 0.25f);
                }

                flash = 1;
                death_sound.Play();
                iframes = 15f;
            }
        }

        if (collision.tag == "EnemyBullet")
        {
            if (iframes <= 0f)
            {
                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 8f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 8f;
                if (GameObject.Find("PlayerHealth") != null) GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().scale[GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().hp-1] = 0;
                if (GameObject.Find("PlayerHealth") != null) GameObject.Find("PlayerHealth").GetComponent<PlayerHealthCode>().hp--;

                GameObject _e1 = Instantiate(explosion, transform.position, Quaternion.identity);
                _e1.GetComponent<ExplosionCode>().timer = 0.25f;
                _e1.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.1f);
                _e1.GetComponent<ExplosionCode>().create_times = 0;
                _e1.GetComponent<SpriteRenderer>().sortingOrder = 12;
                _e1.GetComponent<ExplosionCode>().scale = 12f;
                _e1.GetComponent<ExplosionCode>().un_timed = true;

                for (int _i = 0; _i < 3; _i++)
                {
                    GameObject _e = Instantiate(explosion, transform.position, Quaternion.identity);
                    _e.GetComponent<ExplosionCode>().create_times = Random.Range(3, 7);
                    _e.GetComponent<ExplosionCode>().dir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized;
                    _e.GetComponent<ExplosionCode>().timer = Random.Range(-0.1f, 0.25f);
                }

                flash = 1;
                death_sound.Play();
                Destroy(collision.gameObject);

                iframes = 15f;
            }
        }
    }
}
