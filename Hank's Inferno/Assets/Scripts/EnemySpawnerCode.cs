using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerCode : MonoBehaviour
{
    public GameObject enemy;
    private float timer = 12f;
    private float rotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        rotation += 5f;

        timer -= 10f * Time.deltaTime;

        if(timer <= 0)
        {
            Instantiate(enemy, transform.position, Quaternion.identity);
            if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 3f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 3f;
            Destroy(gameObject);
        }
    }
}
