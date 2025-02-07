using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomData : MonoBehaviour
{
    private BoxCollider2D trigger;
    [SerializeField] bool withdrawlsDisabled = false;

    // Start is called before the first frame update
    void Awake()
    {
        trigger = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            // Allt detta gör är att ge rummets yta till kamera skriptet så att den håller sig där.
            Camera.main.GetComponent<CameraController>().cameraBounds = new Rect(transform.position.x - (trigger.size.x / 2), transform.position.y + (trigger.size.y / 2), transform.position.x + (trigger.size.x / 2), transform.position.y - (trigger.size.y / 2));

            collision.GetComponent<PlayerMovement>().withdrawls = !withdrawlsDisabled;
        }
    }
}
