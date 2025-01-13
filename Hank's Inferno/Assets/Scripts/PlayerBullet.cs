using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 0f;
    public float drag = 1f;
    public float lifetime = 60;
    public float knockback = 4;
    public float damage = 1;
    public bool hitscan = false;

    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.right * speed * Time.fixedDeltaTime;
        speed = Mathf.Max(speed - drag * Time.fixedDeltaTime, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }
}
