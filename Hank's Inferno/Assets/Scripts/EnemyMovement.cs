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

    // Start is called before the first frame update
    void Start()
    {
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
                            fake_sprite.GetComponent<Animator>().Play("EnemyBasicSlide",0,0);
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
                            fake_sprite.GetComponent<Animator>().Play("EnemyBasicSlide",0,0);
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
                        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 1.25f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 1.25f;

                        GameObject _bullet = Instantiate(enemy_bullet, transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);
                        _bullet.GetComponent<EnemyBulletCode>().direction = player.transform.position - transform.position;

                        bullet_timer = 8f;
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
            GameObject.Find("Player").GetComponent<PlayerMovement>().enemies_to_kill--;
            Destroy(gameObject);
        }
    }
}
