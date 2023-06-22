using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    public float speed = 1.0f;
    public float amplitude = 0.5f;

    private float startY;   

    bool goingUp = true;
    private void Start()
    {
        startY = transform.position.y;
    }
    private void Update()
    {
        if(goingUp)
        {
            transform.position = new Vector2(transform.position.x, transform.position.y + Time.deltaTime * speed);
            if(transform.position.y >= startY + amplitude)
            {
                goingUp = false;
            }
        }
        else
        {
            transform.position = new Vector2(transform.position.x, transform.position.y - Time.deltaTime * speed);
            if (transform.position.y <= startY - amplitude)
            {
                goingUp = true;
            }
        }
    }
}
