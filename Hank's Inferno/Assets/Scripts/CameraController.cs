using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Initializing variables
    public GameObject player;
    public float screen_shake = 0;
    public float camera_y = 0f;
    public SpriteRenderer transition;
    public float alpha = 1;
    public bool fade = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float _y_max = player.transform.position.y;
        _y_max = Mathf.Clamp(_y_max, camera_y - 2f, camera_y + 2f);

        // Movement
        transform.position += ((new Vector3(player.transform.position.x, _y_max, player.transform.position.z) + new Vector3(Random.Range(-screen_shake,screen_shake), Random.Range(-screen_shake, screen_shake), 0)) - transform.position) * 10f * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);

        // Screen Shake
        screen_shake += (0 - screen_shake) * 10f * Time.deltaTime;

        // Transitions
        transition.color = new Color(0, 0, 0, alpha);

        if(fade == true)
        {
            alpha += 5f * Time.deltaTime;
        }
        else
        {
            alpha -= 5f * Time.deltaTime;
        }

        alpha = Mathf.Clamp(alpha, 0, 1f);
    }
}
