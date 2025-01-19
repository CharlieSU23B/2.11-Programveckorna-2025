using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCode : MonoBehaviour
{
    public Texture2D tex;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        Cursor.SetCursor(tex, new Vector2(64,64), CursorMode.ForceSoftware);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
