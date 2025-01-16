using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public float speed = 5f;
    public float leftEdge = -10f;
    public float rightEdge = 10f;

    private int direction = 1;

    void Update()
    {
        transform.Translate(Vector2.right * speed * direction * Time.deltaTime);

        if (transform.position.x >= rightEdge || transform.position.x <= leftEdge)
        {
            direction *= -1;
        }
    }
}
