using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildlifeCode : MonoBehaviour
{
    public float rot;
    public float rot_dir = 1;
    public float rot_dir_time = 0;
    public float rot_wiggle = 0;

    // Start is called before the first frame update
    void Start()
    {
        rot_dir = Random.Range(0.5f, 5f);

        float _scale = Random.Range(1.85f, 2.15f);
        float _x_scale = 1;
        if (Random.Range(0, 100) < 50) _x_scale = -1;
        transform.localScale = new Vector3(_scale*_x_scale,_scale, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(rot_dir_time <= 0f)
        {
            rot_dir = -rot_dir;
            rot_dir_time = 22f;
        }

        rot_dir_time -= 10 * Time.deltaTime;

        rot += ((rot_dir * 25 - rot) * (0.1f * Time.deltaTime));

        transform.rotation = Quaternion.Euler(0, 0, rot);

        if(Vector3.Distance(GameObject.Find("Player").transform.position,transform.position + new Vector3(0,0.5f,0))< 0.5f)
        {
            if (rot_wiggle <= 0)
            {
                rot_wiggle = 1;

                transform.localScale = new Vector3(1, 4f, 1f);
            }
        }
        else
        {
            rot_wiggle = 0;
        }

        transform.localScale += (new Vector3(2, 2, 1) - transform.localScale) * 10f * Time.deltaTime;
    }
}
