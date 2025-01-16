using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 0f;
    public float drag = 1f;
    public float lifetime = 1;
    public float knockback = 4;
    public float damage = 1;
    public bool hitscan = false;
    public GameObject dust;
    public float target_scale = 0.5f;

    private void Start()
    {
        transform.localScale = new Vector3(target_scale * 4, target_scale * 4, 1);
        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 1f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 1f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.localScale += (new Vector3(target_scale, target_scale, 1) - transform.localScale) * 10f * Time.deltaTime;
        transform.position += transform.right * speed * Time.fixedDeltaTime;
        speed = Mathf.Max(speed - drag * Time.fixedDeltaTime, 0f);

        lifetime -= 10f * Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            GameObject _dust = Instantiate(dust, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }

        if(collision.tag == "Enemy")
        {
            if (collision.GetComponent<EnemyMovement>() != null)
            {
                collision.GetComponent<EnemyMovement>().hp -= damage;
                collision.GetComponent<EnemyMovement>().flash = 1;
                GameObject.Find("Player").GetComponent<PlayerMovement>().hurt_sound.Play();
                float _h_recoil = (collision.transform.position.x - transform.position.x);

                if (_h_recoil >= 0) _h_recoil = 1;
                else _h_recoil = -1;

                float _v_recoil = (collision.transform.position.y - transform.position.y);

                if (_v_recoil >= 0) _v_recoil = 1;
                else _v_recoil = -1;

                collision.GetComponent<EnemyMovement>().h_speed += _h_recoil * 30f;
                collision.GetComponent<EnemyMovement>().v_speed += _v_recoil * -20f;
                collision.GetComponent<EnemyMovement>().x_scale = 0.5f;
                collision.GetComponent<EnemyMovement>().y_scale = 1.5f;

                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 4f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 4f;

                GameObject.Find("Player").GetComponent<PlayerMovement>().healing += 0.0025f*damage;

                Destroy(gameObject);
            }

            if (collision.GetComponent<EnemyMovement2>() != null)
            {
                collision.GetComponent<EnemyMovement2>().hp -= damage;
                collision.GetComponent<EnemyMovement2>().flash = 1;
                GameObject.Find("Player").GetComponent<PlayerMovement>().hurt_sound.Play();
                float _h_recoil = (collision.transform.position.x - transform.position.x);

                if (_h_recoil >= 0) _h_recoil = 1;
                else _h_recoil = -1;

                float _v_recoil = (collision.transform.position.y - transform.position.y);

                if (_v_recoil >= 0) _v_recoil = 1;
                else _v_recoil = -1;

                collision.GetComponent<EnemyMovement2>().h_speed += _h_recoil * 30f;
                collision.GetComponent<EnemyMovement2>().v_speed += _v_recoil * -20f;
                collision.GetComponent<EnemyMovement2>().x_scale = 0.5f;
                collision.GetComponent<EnemyMovement2>().y_scale = 1.5f;

                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 4f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 4f;

                GameObject.Find("Player").GetComponent<PlayerMovement>().healing += 0.0025f * damage;

                Destroy(gameObject);
            }

            if (collision.GetComponent<EnemyMovement3>() != null)
            {
                collision.GetComponent<EnemyMovement3>().hp -= damage;
                collision.GetComponent<EnemyMovement3>().flash = 1;
                GameObject.Find("Player").GetComponent<PlayerMovement>().hurt_sound.Play();
                float _h_recoil = (collision.transform.position.x - transform.position.x);

                if (_h_recoil >= 0) _h_recoil = 1;
                else _h_recoil = -1;

                float _v_recoil = (collision.transform.position.y - transform.position.y);

                if (_v_recoil >= 0) _v_recoil = 1;
                else _v_recoil = -1;

                collision.GetComponent<EnemyMovement3>().h_speed += _h_recoil * 30f;
                collision.GetComponent<EnemyMovement3>().v_speed += _v_recoil * -20f;
                collision.GetComponent<EnemyMovement3>().x_scale = 0.5f;
                collision.GetComponent<EnemyMovement3>().y_scale = 1.5f;

                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 4f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 4f;

                GameObject.Find("Player").GetComponent<PlayerMovement>().healing += 0.0025f * damage;

                Destroy(gameObject);
            }

            if (collision.GetComponent<BossMovement>() != null)
            { 
                collision.GetComponent<BossMovement>().hp -= damage;
                collision.GetComponent<BossMovement>().flash = 1;
                GameObject.Find("Player").GetComponent<PlayerMovement>().hurt_sound.Play();
                if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 4f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 4f;

                GameObject.Find("Player").GetComponent<PlayerMovement>().healing += 0.0025f * damage;

                Destroy(gameObject);
            }
        }
    }
}
