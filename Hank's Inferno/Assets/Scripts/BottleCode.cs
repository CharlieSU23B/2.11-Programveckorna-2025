using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottleCode : MonoBehaviour
{
    public Image border;
    public Image fill;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        fill.fillAmount = GameObject.Find("Player").GetComponent<PlayerMovement>().healing_draw;
    }
}
