using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class EnemyMovement2 : MonoBehaviour
{
    // Initializing variables
    public Rigidbody2D rb;
    public float h_speed;
    private float max_speed = 7f;
    private bool grounded = false;
    public LayerMask ground_layer;
    public float box_distance = 0;
    public Vector2 box_mask;
    public float v_speed;
    public float x_scale = 0f;
    public float y_scale = 0f;
    public GameObject fake_sprite;
    public string state = "FREE";
    private float walk_dir = 1;
    public GameObject player;
    private float rush_timer = 0;
    public GameObject dust;
    public float hp = 90;
    public GameObject explosion;

    public float flash = 0;
    public SpriteRenderer flash_sprite;

    // Start is called before the first frame update
    void Start()
    {
        hp = 90;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(state)
        {
            case ("FREE"):
                {
                    fake_sprite.GetComponent<Animator>().Play("RushEnemyDash",0,0);

                    // Walking code
                    if(player.transform.position.x > transform.position.x)
                    {
                        walk_dir = 1;
                    }
                    else
                    {
                        walk_dir = -1;
                    }

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

                            Instantiate(dust, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);

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

                    // Wall Check
                    if (Physics2D.BoxCast(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.05f, 0.4f), 0, -transform.right, 0.6f, ground_layer) && walk_dir == -1)
                    {
                        if (grounded)
                        {
                            x_scale = 0.5f;
                            y_scale = 1.5f;
                            v_speed = 25f;
                            grounded = false;
                        }
                    }

                    if (Physics2D.BoxCast(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.05f, 0.4f), 0, transform.right, 0.6f, ground_layer) && walk_dir == 1)
                    {
                        if (grounded)
                        {
                            x_scale = 0.5f;
                            y_scale = 1.5f;
                            v_speed = 25f;
                            grounded = false;
                        }
                    }

                    if(grounded && Vector3.Distance(transform.position,player.transform.position) < 5f)
                    {
                        if(!Physics2D.BoxCast(transform.position + new Vector3(0, 0.5f, 0), new Vector2(5, 0.4f), 0, transform.right*walk_dir, 0.6f, ground_layer))
                        {
                            h_speed = 0;

                            fake_sprite.GetComponent<Animator>().Play("RushEnemyDash",0,0);

                            state = "RUSH";

                            rush_timer = 6.5f;

                            x_scale = 0.25f;
                            y_scale = 1.75f;
                        }
                    }

                    h_speed = Mathf.Clamp(h_speed, -30, 30);

                    // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale * walk_dir * -6, y_scale * 6);
                    fake_sprite.transform.position = new Vector3(transform.position.x-(0.75f*walk_dir),transform.position.y,transform.position.z);
                }
                break;

            case ("RUSH"):
                {
                    // Walking code
                    if (rush_timer <= 2.5f)
                    {
                        if (walk_dir != 0)
                        {
                            h_speed += ((60f * walk_dir) - h_speed) * 40f * Time.deltaTime;
                            x_scale = 2f;

                            if (GameObject.Find("Main Camera"))
                            {
                                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 0.625f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 0.625f;
                            }
                        }
                        else
                        {
                            h_speed += (0 - h_speed) * 18f * Time.deltaTime;
                        }
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

                            Instantiate(dust, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);

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

                    // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale * walk_dir * -6, y_scale * 6);
                    fake_sprite.transform.position = new Vector3(transform.position.x - (0.75f * walk_dir), transform.position.y, transform.position.z);

                    if (rush_timer <= 0)
                    {
                        h_speed = 0;
                        state = "FREE";
                    }
                }
                break;
        }

        flash_sprite.color = new Color(1, 1, 1, flash);
        flash -= 0.1f;
        flash = Mathf.Clamp(flash, 0, 1);

        if (hp <= 0)
        {
            if (Random.Range(0, 100) <= 30)
            {
                GameObject.Find("Hank_arm").GetComponent<GamblingGun>().coin.transform.localScale = new Vector3(0, 0, 0);
                GameObject.Find("Hank_arm").GetComponent<GamblingGun>().coins_amount++;
            }

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
            GameObject.Find("Player").GetComponent<PlayerMovement>().enemies_to_kill--;
            GameObject.Find("Player").GetComponent<PlayerMovement>().death_sound.Play();
            Destroy(gameObject);
        }

        rush_timer -= 10f * Time.deltaTime;
    }
}