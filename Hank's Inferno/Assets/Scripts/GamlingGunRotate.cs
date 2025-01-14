using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamlingGunRotate : MonoBehaviour
{
    [SerializeField] private Vector3 centerOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion rot = Quaternion.Euler(0, 0, (180 / Mathf.PI) * Mathf.Atan2(Camera.main.ScreenToWorldPoint(Input.mousePosition).y - (transform.position.y + centerOffset.y), Camera.main.ScreenToWorldPoint(Input.mousePosition).x - (transform.position.x + centerOffset.x)));

        float _scale = 1;

        if(Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x)
        {
            _scale = 1;
        }
        else
        {
            _scale = -1;
        }

        transform.localScale = new Vector3(0.4444445f * GameObject.Find("Player").GetComponent<PlayerMovement>().flip_scale, 0.4444445f* _scale, transform.localScale.z);

        transform.rotation = rot;
    }
}
