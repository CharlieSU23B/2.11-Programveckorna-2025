using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DoorCode : MonoBehaviour
{
    // This code is not actually for the doors, it's for the entire elevator,
    // I accidentally named it "DoorCode" and could not change it back without risking merge conflict.

    // Initializing variables
    public GameObject door_1;
    public GameObject door_2;
    public Vector3[] door_pos = new Vector3[99];
    public Vector3[] door_pos_2 = new Vector3[99];
    private bool open = false;
    public GameObject door_place;

    // Start is called before the first frame update

    void Start()
    {
        for (var _i = 0; _i < 11; _i++)
        {
            Instantiate(door_place, door_pos[_i], Quaternion.identity);
            Instantiate(door_place, door_pos_2[_i] + new Vector3(0,-2f,0), Quaternion.identity);
        }
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
                    // Door opens when player enters
                    door_1.transform.position += (new Vector3(transform.position.x + 2f, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
                else
                {
                    // Door closes after player enters

                    door_1.transform.position += (new Vector3(transform.position.x + 1, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 1, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
            }
            else
            {
                // Door stays closed

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
                    if(open == false)
                    {
                        GameObject.Find("Player").GetComponent<PlayerMovement>().elevator_sound.Play();
                        open = true;
                    }

                    // Door opens when player exits

                    door_1.transform.position += (new Vector3(transform.position.x + 2f, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 2f, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
                else
                {
                    // Door stays closed before player exists

                    door_1.transform.position += (new Vector3(transform.position.x + 1, transform.position.y, transform.position.z) - door_1.transform.position) * 10f * Time.deltaTime;
                    door_2.transform.position += (new Vector3(transform.position.x - 1, transform.position.y, transform.position.z) - door_2.transform.position) * 10f * Time.deltaTime;
                }
            }
            else
            {
                // Door closes instantly... for some reason I made this instant

                door_1.transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
                door_2.transform.position = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            }
        }

        // Deptch check, doors are behind player when the elevator is open, over the player when the elevator closes
        if(GameObject.Find("Player").GetComponent<PlayerMovement>().state != "ELEVATOR OPEN" && GameObject.Find("Player").GetComponent<PlayerMovement>().state != "ELEVATOR")
        {
            door_1.GetComponent<SpriteRenderer>().sortingOrder = 0;
            door_2.GetComponent<SpriteRenderer>().sortingOrder = 0;

            open = false;

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
        // Trigger for openning the doors
        if(collision.tag == "Player")
        {
            if(Input.GetKey(KeyCode.E) && collision.GetComponent<PlayerMovement>().state == "FREE" && collision.GetComponent<PlayerMovement>().enemies_to_kill <= 0)
            {
                GameObject.Find("Player").GetComponent<PlayerMovement>().elevator_sound.Play();
                collision.GetComponent<PlayerMovement>().state = "ELEVATOR";
                collision.GetComponent<PlayerMovement>().elevator = gameObject;
                collision.GetComponent<PlayerMovement>().elevator_timer = 18f;
            }
        }
    }
}
