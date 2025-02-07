using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodIsHere : MonoBehaviour
{
    // Detta skript �r v�ldigt snabbt gjort

    [SerializeField] Transform god;

    bool isHidden = true;
    float introAnim = 1f, waitTime = 1f;
    float value = 0;
    Vector3 ogPos;
    PlayerMovement player;

    // Start is called before the first frame update
    void Awake()
    {
        ogPos = god.position;
        god.position = ogPos + Vector3.down * 20f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHidden)
            return;

        if (introAnim > 0f)
        {
            // Emil kikar upp
            introAnim -= Time.deltaTime * 0.3f;
            god.position = ogPos + Vector3.down * (Mathf.Clamp(introAnim, 0f, 1f) * 20f);
        }
        else if (waitTime > 0f)
        {
            // V�nta lite kort
            waitTime -= Time.deltaTime;
            if (waitTime <= 0f)
            {
                player.state = "FREE";
            }
        }
        else
        {
            // B�rja �ka fram och tillbaka
            value += Time.deltaTime;
            god.position = ogPos + new Vector3(Mathf.Sin(Mathf.Deg2Rad * value * 50f) * 6f, Mathf.Sin(Mathf.Deg2Rad * value * 250f) * 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Om spelaren nuddar triggern s� aktiveras Emil grejerna
        if (collision.tag == "Player" && isHidden)
        {
            isHidden = false;
            player = collision.GetComponent<PlayerMovement>();

            player.state = "EMPTY";
        }
    }
}
