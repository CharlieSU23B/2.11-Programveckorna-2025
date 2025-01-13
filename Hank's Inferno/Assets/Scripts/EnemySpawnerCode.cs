using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerCode : MonoBehaviour
{
    public GameObject[] enemy = new GameObject[99];
    private float timer = 25f;
    private float rotation = 0;
    public float camera_y = 0;
    public int enemy_index = 0;
    public bool enemy_kill = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        rotation += 5f;

        if (camera_y != GameObject.Find("Main Camera").GetComponent<CameraController>().camera_y)
        {
            timer = 25f;
        }
        else
        {
            if (enemy_kill == false)
            {
                GameObject.Find("Player").GetComponent<PlayerMovement>().enemies_to_kill++;

                enemy_kill = true;
            }
        }

        timer -= 10f * Time.deltaTime;

        if(timer <= 0)
        {
            Instantiate(enemy[enemy_index], transform.position, Quaternion.identity);
            if (GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake < 3f) GameObject.Find("Main Camera").GetComponent<CameraController>().screen_shake = 3f;
            Destroy(gameObject);
        }
    }
}
