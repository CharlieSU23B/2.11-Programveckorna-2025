using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorCode : MonoBehaviour
{
    // Initializing variables
    public GameObject door_1;
    public GameObject door_2;
    public Vector3[] door_pos = new Vector3[99];
    public Vector3[] door_pos_2 = new Vector3[99];

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
                    door_1.transform.position += (new Vector3(transform.position.x + 2f, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
                else
                {
                    door_1.transform.position += (new Vector3(transform.position.x + 1, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 1, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
            }
            else
            {
                door_1.transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                door_2.transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            }
        }
        else
        {
            if (GameObject.Find("Player").GetComponent<PlayerMovement>().elevator_timer > 0)
            {
                if (GameObject.Find("Player").GetComponent<PlayerMovement>().elevator_timer <= 10)
                {
                    door_1.transform.position += (new Vector3(transform.position.x + 2f, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
                else
                {
                    door_1.transform.position += (new Vector3(transform.position.x + 1, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 1, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
            }
            else
            {
                door_1.transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                door_2.transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            }
        }

        if(GameObject.Find("Player").GetComponent<PlayerMovement>().state != "ELEVATOR OPEN" && GameObject.Find("Player").GetComponent<PlayerMovement>().state != "ELEVATOR")
        {
            door_1.GetComponent<SpriteRenderer>().sortingOrder = 0;
            door_2.GetComponent<SpriteRenderer>().sortingOrder = 0;

            transform.position += (door_pos_2[GameObject.Find("Player").GetComponent<PlayerMovement>().room_i] - transform.position) * 2f * Time.deltaTime;
        }
        else
        {
            door_1.GetComponent<SpriteRenderer>().sortingOrder = 4;
            door_2.GetComponent<SpriteRenderer>().sortingOrder = 4;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(Input.GetKey(KeyCode.E) && collision.GetComponent<PlayerMovement>().state == "FREE" && collision.GetComponent<PlayerMovement>().enemies_to_kill <= 0)
            {
                collision.GetComponent<PlayerMovement>().state = "ELEVATOR";
                collision.GetComponent<PlayerMovement>().elevator = gameObject;
                collision.GetComponent<PlayerMovement>().elevator_timer = 18f;
            }
        }
    }
}
