using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletCode : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector3 direction;
    private float range = 3f;
    public float target_scale = 0.25f;
    public string creator_string;
    public float b_speed = 12f;
    public float angle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        transform.localScale = new Vector3(target_scale*8, target_scale*8, 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector2(direction.x, direction.y).normalized * b_speed;

        transform.rotation = Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg);

        transform.localScale += (new Vector3(target_scale, target_scale, 1) - transform.localScale) * 10f * Time.deltaTime;

        range -= Time.deltaTime;

        if(range <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground") Destroy(gameObject);
    }
}
