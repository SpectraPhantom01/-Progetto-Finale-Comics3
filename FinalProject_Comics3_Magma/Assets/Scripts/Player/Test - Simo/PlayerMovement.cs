using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movemens Settings")]
    [Tooltip("Velocità massima del player")]
    [SerializeField] float maxSpeed = 6.5f;
    [Tooltip("Accelerazione del player")]
    [SerializeField] float acceleration = 25;
    [Tooltip("Decelerazione del player")]
    [SerializeField] float deceleration = 35;

    [HideInInspector] public Vector2 Direction;

    //InputSystem inputSystem;
    
    Vector2 movement;
    Rigidbody2D rb;

    bool changingDirectionX => (rb.velocity.x > 0f && Direction.x < 0f) || (rb.velocity.x < 0f && Direction.x > 0f);
    bool changingDirectionY => (rb.velocity.y > 0f && Direction.y < 0f) || (rb.velocity.y < 0f && Direction.y > 0f);

    private void Awake()
    {
        //inputSystem = new InputSystem();
        rb = GetComponentInChildren<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        //Direction = inputSystem.Player.Movement.ReadValue<Vector2>(); // Direction salva i valori di movimento presi da input
        MoveDirection(); // Calcolo del movement applicando accelerazione e decelerazione su assi x e y
        
        rb.velocity = new Vector2(movement.x, movement.y); // Movimento effettivo
    }

    private void MoveDirection() // NOTA: aggiunto controllo sul cambio di direzione,
                            // per evitare un movimento "scivoloso" quando avviene un cambio di direzione opposto
    {
        if (Mathf.Abs(Direction.x) < 0.01f || changingDirectionX) // Applicazione decelerazione su asse x
        {
            movement.x = Mathf.MoveTowards(movement.x, 0, deceleration * Time.fixedDeltaTime);
        }
        else // Applicazione accelerazione su asse x e clamp sulla massima velocità impostata
        {
            movement.x += Direction.x * acceleration * Time.fixedDeltaTime;
            movement.x = Mathf.Clamp(movement.x, -maxSpeed, maxSpeed);
        }

        if (Mathf.Abs(Direction.y) < 0.01f || changingDirectionY) // Applicazione decelerazione su asse y
        {
            movement.y = Mathf.MoveTowards(movement.y, 0, deceleration * Time.fixedDeltaTime);
        }
        else // Applicazione accelerazione su asse y e clamp sulla massima velocità impostata
        {
            movement.y += Direction.y * acceleration * Time.fixedDeltaTime;
            movement.y = Mathf.Clamp(movement.y, -maxSpeed, maxSpeed);
        }

    }

    //private void OnEnable()
    //{
    //    inputSystem.Player.Enable();
    //}

    //private void OnDisable()
    //{
    //    inputSystem.Player.Disable();
    //}
}
