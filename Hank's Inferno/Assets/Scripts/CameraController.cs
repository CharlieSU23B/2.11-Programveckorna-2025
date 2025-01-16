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
    public SpriteRenderer screen_flash;
    public float flash_alpha = 0f;

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
        float _screen_shake = screen_shake * 0.7f;

        transform.position += ((new Vector3(player.transform.position.x, _y_max, player.transform.position.z) + new Vector3(Random.Range(-_screen_shake, _screen_shake), Random.Range(-_screen_shake, _screen_shake), 0)) - transform.position) * 10f * Time.deltaTime;
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

        // Screen Flash Red
        flash_alpha -= 5f * Time.deltaTime;
        flash_alpha = Mathf.Clamp(flash_alpha, 0, 1f);
        screen_flash.color = new Color(1f, 0, 0, flash_alpha);
    }
}
