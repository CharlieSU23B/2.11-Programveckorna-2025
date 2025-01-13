using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 0f;
    public float drag = 1f;
    public float lifetime = 60;
    public float knockback = 4;
    public float damage = 1;
    public bool hitscan = false;
    public GameObject dust;
    public float target_scale = 0.5f;

    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, lifetime); 
    }

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
            }

            if (collision.GetComponent<EnemyMovement2>() != null)
            {
                collision.GetComponent<EnemyMovement2>().hp -= damage;
                collision.GetComponent<EnemyMovement2>().flash = 1;
            }

            if (collision.GetComponent<EnemyMovement3>() != null)
            {
                collision.GetComponent<EnemyMovement3>().hp -= damage;
                collision.GetComponent<EnemyMovement3>().flash = 1;
            }

            if (collision.GetComponent<BossMovement>() != null)
            {
                collision.GetComponent<BossMovement>().hp -= damage;
                collision.GetComponent<BossMovement>().flash = 1;
            }

            if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 4f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 4f;

            Destroy(gameObject);
        }
    }
}
