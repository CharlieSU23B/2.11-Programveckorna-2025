using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // Initializing variables
    public Rigidbody2D rb;
    public float h_speed;
    private float max_speed = 5.5f;
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
    public float hp = 70;
    public float flash = 0;
    public SpriteRenderer flash_sprite;
    private bool shoot = false;
    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        hp = 70;

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
                    if (Physics2D.BoxCast(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.05f, 0.4f), 0, -transform.right, 1.4f, ground_layer))
                    {
                        if (walk_dir == -1)
                        {
                            x_scale = 0.75f;
                            y_scale = 1.25f;
                            h_speed *= 2;
                        }

                        walk_dir = 1;
                    }

                    if (Physics2D.BoxCast(transform.position + new Vector3(0, 0.5f, 0), new Vector2(0.05f, 0.4f), 0, transform.right, 1.4f, ground_layer))
                    {
                        if (walk_dir == 1)
                        {
                            x_scale = 0.75f;
                            y_scale = 1.25f;
                            h_speed *= 2;
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
                        fake_sprite.GetComponent<Animator>().Play("EnemyBasicSlide", 0, 0);

                        shoot = false;

                        bullet_timer = 8f;
                    }

                    if(bullet_timer <= 7f && shoot == false)
                    {
                        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 1.25f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 1.25f;

                        GameObject _bullet = Instantiate(enemy_bullet, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                        _bullet.GetComponent<EnemyBulletCode>().direction = player.transform.position - transform.position;
                        _bullet.GetComponent<EnemyBulletCode>().angle = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x);

                        x_scale = 1.75f;
                        y_scale = 0.25f;

                        shoot = true;
                    }

                    if (bullet_timer <= 3f)
                    {
                        fake_sprite.GetComponent<Animator>().Play("EnemyBasicSlide", 0, 0);
                    }

                    bullet_timer -= 10f * Time.deltaTime;

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

                    h_speed = Mathf.Clamp(h_speed, -30, 30);

                     // Rigidbody
                    rb.velocity = new Vector2(h_speed, v_speed);

                    // Squash and Stretch
                    x_scale += (1 - x_scale) * 10f * Time.deltaTime;
                    y_scale += (1 - y_scale) * 10f * Time.deltaTime;

                    fake_sprite.transform.localScale = new Vector2(x_scale*walk_dir*-4, y_scale*4);
                }
                break;
        }

        flash_sprite.color = new Color(1, 1, 1, flash);
        flash -= 0.1f;
        flash = Mathf.Clamp(flash, 0, 1);

        flash_sprite.sprite = fake_sprite.GetComponent<SpriteRenderer>().sprite;

        if(hp <= 0)
        {
            if(Random.Range(0,100) <= 30)
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
    }
}
