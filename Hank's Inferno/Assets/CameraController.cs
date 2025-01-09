using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Initializing variables
    public GameObject player;
    public float screen_shake = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Movement
        transform.position += ((player.transform.position + new Vector3(Random.Range(-screen_shake,screen_shake), Random.Range(-screen_shake, screen_shake), 0)) - transform.position) * 10f * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y+0.2f, -10);

        // Screen Shake
        screen_shake += (0 - screen_shake) * 10f * Time.deltaTime;
    }
}
