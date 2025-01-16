using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class EnemyMovement3 : MonoBehaviour
{
    // Initializing variables
    public Rigidbody2D rb;
    public float h_speed;
    private float max_speed = 8f;
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
    public GameObject enemy_bullet;
    public GameObject player;
    private float bullet_timer = 0;
    public GameObject dust;
    public float hp = 35;
    public GameObject explosion;

    public float flash = 0;
    public SpriteRenderer flash_sprite;

    // Start is called before the first frame update
    void Start()
    {
        hp = 35;
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(state)
        {
            case ("FREE"):
                {
                    // Wall Check
                    if (player.transform.position.x > transform.position.x)
                    {
                        walk_dir = 1;
                    }

                    if (player.transform.position.x < transform.position.x)
                    {
                        walk_dir = -1;
                    }

                    // Walking code
                    if (walk_dir != 0)
                    {
                        h_speed += ((max_speed * walk_dir) - h_speed) * 2f * Time.deltaTime;
                    }
                    else
                    {
                        h_speed += (0 - h_speed) * 18f * Time.deltaTime;
                    }

                    if(Vector3.Distance(transform.position,player.transform.position) < 25f && bullet_timer <= 0f)
                    {
                        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 1.25f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 1.25f;

                        x_scale = 0.5f;
                        y_scale = 1.5f;

                        GameObject _bullet1 = Instantiate(enemy_bullet, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                        _bullet1.GetComponent<EnemyBulletCode>().direction = (player.transform.position - transform.position) + new Vector3(1.5f,1.5f,0);

                        GameObject _bullet2 = Instantiate(enemy_bullet, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                        _bullet2.GetComponent<EnemyBulletCode>().direction = (player.transform.position - transform.position) + new Vector3(-1.5f, 1.5f, 0);

                        GameObject _bullet3 = Instantiate(enemy_bullet, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                        _bullet3.GetComponent<EnemyBulletCode>().direction = (player.transform.position - transform.position);

                        bullet_timer = 16f;
                    }

                    bullet_timer -= 10f * Time.deltaTime;

                    // Ground check
                    if (Physics2D.BoxCast(transform.position, box_mask, 0, -transform.up, box_distance, ground_layer))
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
                        v_speed = 5f;
                    }
                    else
                    {
                        grounded = false;

                        // Flying code
                        if (transform.position.y >= player.transform.position.y + 5f)
                        {
                            v_speed -= 15f * Time.deltaTime;
                        }
                        else
                        {
                            v_speed += 20f * Time.deltaTime;
                        }

                        v_speed = Mathf.Clamp(v_speed, -5f, 5f);
                    }

                    h_speed = Mathf.Clamp(h_speed, -30, 30);

                    // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale, y_scale);
                }
                break;
        }

        flash_sprite.color = new Color(1, 1, 1, flash);
        flash -= 0.1f;
        flash = Mathf.Clamp(flash, 0, 1);

        if (hp <= 0)
        {
            for (int _i = 0; _i < 3; _i++)
            {
                GameObject _e = Instantiate(explosion, transform.position, Quaternion.identity);
                _e.GetComponent<ExplosionCode>().create_times = Random.Range(3, 7);
                _e.GetComponent<ExplosionCode>().dir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized;
            }
            GameObject.Find("Player").GetComponent<PlayerMovement>().enemies_to_kill--;

            Destroy(gameObject);
        }
    }
}
