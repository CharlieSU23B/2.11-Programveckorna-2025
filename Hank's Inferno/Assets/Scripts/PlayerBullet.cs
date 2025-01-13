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
    public GameObject dust;
    public Quaternion rot;
    public float target_scale = 1;

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
            GameObject _dust = Instantiate(dust, transform.position, Quaternion.Euler(rot.x, rot.y, rot.z+90));
            _dust.transform.localScale = new Vector3(0.5f, 1f, 1f);

            Destroy(gameObject);
        }
    }
}
