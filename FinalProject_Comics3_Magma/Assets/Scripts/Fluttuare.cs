using UnityEngine;

public class Fluttuare : MonoBehaviour
{
    public float speed = 1.0f;      // velocità di fluttuazione impostabile dall'editor
    public float amplitude = 0.5f;  // ampiezza della fluttuazione

    private float startY;           // posizione Y iniziale del GameObject

    void Start()
    {
        // salviamo la posizione Y iniziale del GameObject
        startY = transform.position.y;
    }

    void Update()
    {
        // calcoliamo la nuova posizione Y del GameObject
        float newY = startY + amplitude * Mathf.Sin(Time.time * speed);

        // aggiorniamo la posizione del GameObject
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
