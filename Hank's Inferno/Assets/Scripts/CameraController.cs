using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Initializing variables
    private Camera cam;
    public GameObject player;
    public float screen_shake = 0;
    public float camera_y = 0f;
    public SpriteRenderer transition;
    public float alpha = 1;
    public bool fade = false;
    public SpriteRenderer screen_flash;
    public float flash_alpha = 0f;
    public bool stripMode = true;

    public Rect cameraBounds = new Rect(-10, 10, 10, -10);
    private Vector3 trueCamPos = new Vector3();


    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stripMode)
        {
            float _y_max = player.transform.position.y;
            _y_max = Mathf.Clamp(_y_max, camera_y - 2f, camera_y + 2f);

            // Movement
            float _screen_shake = screen_shake * 0.7f;

            transform.position += ((new Vector3(player.transform.position.x, _y_max, player.transform.position.z) + new Vector3(Random.Range(-_screen_shake, _screen_shake), Random.Range(-_screen_shake, _screen_shake), 0)) - transform.position) * 10f * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        }
        else
        {
            // Detta är en rum baserad kamera som kan ha alla möjliga storlekar på rummen (inte form dock, det måste vara en rektangel) gjord av Dennis!!!
            Vector3 targetPos = player.transform.position + new Vector3(0, 1, -10);
            float horSize = cam.orthographicSize * (16f / 9f);

            trueCamPos = Vector3.Lerp(trueCamPos, new Vector3(Mathf.Clamp(targetPos.x, cameraBounds.x + horSize, cameraBounds.width - horSize), Mathf.Clamp(targetPos.y, cameraBounds.height + cam.orthographicSize, cameraBounds.y - cam.orthographicSize), -10), 10f * Time.fixedDeltaTime);
            transform.position = trueCamPos + 0.7f * new Vector3(Random.Range(-screen_shake, screen_shake), Random.Range(-screen_shake, screen_shake));
        }

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
