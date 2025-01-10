using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DustCode : MonoBehaviour
{
    private float timer = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 10f * Time.deltaTime;

        if (timer <= 0f) Destroy(gameObject);
    }
}
