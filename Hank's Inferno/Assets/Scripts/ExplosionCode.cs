using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCode : MonoBehaviour
{
    private float timer = 0.25f;
    public int create_times = 1;
    public bool create = false;
    public Vector3 dir;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(-0.1f, 0.25f);

        transform.localScale = new Vector3(0, 0, 0);
        if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 12f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 12f;

        if (create_times <= 0) create = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer <= 0)
        {
            if(create == false)
            {
                GameObject _e = Instantiate(gameObject, transform.position + new Vector3(2*dir.x,2*dir.y,0), Quaternion.identity);
                _e.GetComponent<ExplosionCode>().dir = new Vector3(dir.x + Random.Range(-0.8f,0.8f), dir.y + Random.Range(-0.8f, 0.8f),0).normalized;
                _e.GetComponent<ExplosionCode>().create_times = create_times-1;

                create = true;
            }

            transform.localScale -= new Vector3(10, 10, 10) * Time.deltaTime;

            if (transform.localScale.x <= 0) Destroy(gameObject);
        }
        else
        {
            transform.localScale += (new Vector3(4f, 4f, 4f) - transform.localScale) * 10f * Time.deltaTime;
        }

        timer -= Time.deltaTime;
    }
}
