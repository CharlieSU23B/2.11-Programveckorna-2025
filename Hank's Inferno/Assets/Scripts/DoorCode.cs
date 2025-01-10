using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorCode : MonoBehaviour
{
    // Initializing variables
    public GameObject door_1;
    public GameObject door_2;

    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameObject.Find("Player").GetComponent<PlayerMovement>().state != "ELEVATOR OPEN")
        {
            if (GameObject.Find("Player").GetComponent<PlayerMovement>().elevator_timer > 0)
            {
                if (GameObject.Find("Player").GetComponent<PlayerMovement>().elevator_timer > 10)
                {
                    door_1.transform.position += (new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
                else
                {
                    door_1.transform.position += (new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
            }
            else
            {
                door_1.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
                door_2.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
            }
        }
        else
        {
            if (GameObject.Find("Player").GetComponent<PlayerMovement>().elevator_timer > 0)
            {
                if (GameObject.Find("Player").GetComponent<PlayerMovement>().elevator_timer <= 10)
                {
                    door_1.transform.position += (new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 1f, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
                else
                {
                    door_1.transform.position += (new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
            }
            else
            {
                door_1.transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y, transform.position.z);
                door_2.transform.position = new Vector3(transform.position.x - 0.5f, transform.position.y, transform.position.z);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(Input.GetKey(KeyCode.E))
            {
                collision.GetComponent<PlayerMovement>().state = "ELEVATOR";
                collision.GetComponent<PlayerMovement>().elevator = gameObject;
                collision.GetComponent<PlayerMovement>().elevator_timer = 25f;
            }
        }
    }
}
