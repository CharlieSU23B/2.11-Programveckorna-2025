using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarHide : MonoBehaviour
{
    public float alpha = 0;
    public Image border; 
    public Image fill; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        alpha -= 0.1f;
        alpha = Mathf.Clamp(alpha, 0, 2);

        border.color = new Color(1, 1, 1, alpha);
        fill.color = new Color(1, 1, 1, alpha);
    }
}
