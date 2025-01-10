using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletCode : MonoBehaviour
{
    public Rigidbody2D rb;
    public Vector3 direction;
    private float range = 3f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        transform.localScale = new Vector3(2, 2, 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = new Vector2(direction.x, direction.y).normalized * 12f;

        transform.localScale += (new Vector3(0.25f, 0.25f, 1) - transform.localScale) * 10f * Time.deltaTime;

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
