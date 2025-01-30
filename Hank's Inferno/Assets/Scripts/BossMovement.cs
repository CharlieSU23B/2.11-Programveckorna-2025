using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

public class BossMovement : MonoBehaviour
{
    // Initializing variables
    public Rigidbody2D rb;
    private float h_speed;
    private float max_speed = 5.5f;
    private bool grounded = false;
    public LayerMask ground_layer;
    public float box_distance = 0;
    public Vector2 box_mask;
    private float v_speed;
    private float x_scale = 0f;
    private float y_scale = 0f;
    public GameObject fake_sprite;
    public string state = "FREE";
    private float walk_dir = 1;
    public GameObject enemy_bullet;
    public GameObject player;
    private float bullet_timer = 0;
    public GameObject dust;
    public float hp = 1200;
    public float max_hp = 1200;
    private float draw_hp = 0;
    public GameObject boss_health_bar;
    public Image health_fill;
    public GameObject health_bar;
    //private float wave_timer = 25f;

    public float flash = 0;
    public SpriteRenderer flash_sprite;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        health_bar = GameObject.Find("BossHealthFill");
        boss_health_bar = GameObject.Find("BossHealthBar");
        health_fill = health_bar.GetComponent<Image>();

        hp = 2000;
        max_hp = 2000;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch(state)
        {
            case ("FREE"):
                {
                    if(hp <= max_hp/2)
                    {
                        // Phase 2
                        max_speed = 9.5f;
                    }
                    else
                    {
                        // Phase 1
                        max_speed = 8f;
                    }

                    // Wall Check
                    if (Physics2D.BoxCast(transform.position + new Vector3(0, 2, 0), new Vector2(0.05f, 1.6f), 0, -transform.right, 2.4f, ground_layer))
                    {
                        if (walk_dir == -1)
                        {
                            x_scale = 0.75f;
                            y_scale = 1.25f;
                            h_speed *= -2;
                        }

                        walk_dir = 1;
                    }

                    if (Physics2D.BoxCast(transform.position + new Vector3(0, 2, 0), new Vector2(0.05f, 1.6f), 0, transform.right, 2.4f, ground_layer))
                    {
                        if (walk_dir == 1)
                        {
                            x_scale = 0.75f;
                            y_scale = 1.25f;
                            h_speed *= -2;
                        }

                        walk_dir = -1;
                    }

                    // Walking code
                    if (walk_dir != 0)
                    {
                        h_speed += ((max_speed * walk_dir) - h_speed) * 10f * Time.deltaTime;
                    }
                    else
                    {
                        h_speed += (0 - h_speed) * 18f * Time.deltaTime;
                    }

                    if(Vector3.Distance(transform.position,player.transform.position) < 10f && bullet_timer <= 0f)
                    {
                        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 1.25f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 1.25f;

                        GameObject _bullet = Instantiate(enemy_bullet, transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
                        _bullet.GetComponent<EnemyBulletCode>().direction = ((player.transform.position + new Vector3(Random.Range(-1,1), Random.Range(-1, 1),0)) - transform.position);

                        y_scale = 0.9f;

                        bullet_timer = 1f;
                    }

                    bullet_timer -= 10f * Time.deltaTime;

                    // Super Wave thing (way to broken)
                    /*
                    if (wave_timer <= 0f)
                    {
                        for (int _i = 0; _i < 8; _i++)
                        {
                            if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 3) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 3;
                            GameObject.Find("Main Camera").GetComponent<CameraController>().flash_alpha = 0.5f;

                            GameObject _bullet = Instantiate(enemy_bullet, new Vector3(Random.Range(-28f, 28f), GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y + 14, 0), Quaternion.identity);
                            _bullet.GetComponent<EnemyBulletCode>().direction = new Vector3(0, -1f, 0);
                            _bullet.GetComponent<EnemyBulletCode>().b_speed = 8f;
                            _bullet.GetComponent<EnemyBulletCode>().target_scale = 2;
                        }

                        y_scale = 0.9f;

                        wave_timer = 35f;
                    }

                    wave_timer -= 10f * Time.deltaTime;
                    */
                    // Ground check
                    if (Physics2D.BoxCast(transform.position, box_mask, 0, -transform.up, box_distance, ground_layer) && rb.velocity.y <= 0f)
                    {
                        if (grounded == false)
                        {
                            if (GameObject.Find("Main Camera"))
                            {
                                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 1.5f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 1.5f;
                            }

                            GameObject _bullet = Instantiate(enemy_bullet, new Vector3(transform.position.x, transform.position.y+1.25f, transform.position.z), Quaternion.identity);
                            _bullet.GetComponent<EnemyBulletCode>().direction = new Vector3(1,0,0);
                            _bullet.GetComponent<EnemyBulletCode>().target_scale = 1;
                            _bullet.GetComponent<EnemyBulletCode>().b_speed = 24f;

                            GameObject _bullet2 = Instantiate(enemy_bullet, new Vector3(transform.position.x, transform.position.y+1.25f, transform.position.z), Quaternion.identity);
                            _bullet2.GetComponent<EnemyBulletCode>().direction = new Vector3(-1, 0, 0);
                            _bullet2.GetComponent<EnemyBulletCode>().target_scale = 1;
                            _bullet2.GetComponent<EnemyBulletCode>().b_speed = 24f;

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

                    if(grounded && x_scale <= 1.05f)
                    {
                        v_speed = 25f;

                        if (hp <= max_hp / 2) v_speed = 15f;

                        x_scale = 0.75f;
                        y_scale = 1.25f;
                    }

                     // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale*4, y_scale*4);
                }
                break;
        }

        // Health bar Stuff
        draw_hp += (hp - draw_hp) * 5f * Time.deltaTime;

        health_fill.fillAmount = (draw_hp / max_hp);

        boss_health_bar.GetComponent<HealthBarHide>().alpha += 0.2f;

        flash_sprite.color = new Color(1, 1, 1, flash);
        flash -= 0.1f;
        flash = Mathf.Clamp(flash, 0, 1);

        hp = Mathf.Clamp(hp, 0, max_hp);

        if (hp <= 0)
        {
            GameObject.Find("Player").GetComponent<PlayerMovement>().enemies_to_kill--;
            GameObject.Find("Player").GetComponent<PlayerMovement>().death_sound.Play();
            Destroy(gameObject);
        }
    }
}
