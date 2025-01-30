using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomData : MonoBehaviour
{
    private BoxCollider2D trigger;

    // Start is called before the first frame update
    void Awake()
    {
        trigger = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            Camera.main.GetComponent<CameraController>().cameraBounds = new Rect(transform.position.x - (trigger.size.x / 2), transform.position.y + (trigger.size.y / 2), transform.position.x + (trigger.size.x / 2), transform.position.y - (trigger.size.y / 2));
        }
    }
}
