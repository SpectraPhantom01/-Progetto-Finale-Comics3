using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movemens Settings")]
    [Tooltip("Velocità massima del player")]
    [SerializeField] float maxSpeed = 6.5f;
    [Tooltip("Accelerazione del player")]
    [SerializeField] float acceleration = 25;
    [Tooltip("Decelerazione del player")]
    [SerializeField] float deceleration = 35;

    //Nota: damageAmount e damageableMask aggiunte per test
    [Header("Attacking Settings")]
    [SerializeField] Transform attackPoint;
    [SerializeField] float knockback = 20f;
    [SerializeField] float _damageAmount; 
    [SerializeField] LayerMask _damageableMask;

    [HideInInspector] public Vector2 Direction;
    //[HideInInspector] public GenericStateMachine<EPlayerState> StateMachine;

    //InputSystem inputSystem;

    Vector2 movement;
    float value;
    Rigidbody2D rb;

    bool changingDirectionX => (rb.velocity.x > 0f && Direction.x < 0f) || (rb.velocity.x < 0f && Direction.x > 0f);
    bool changingDirectionY => (rb.velocity.y > 0f && Direction.y < 0f) || (rb.velocity.y < 0f && Direction.y > 0f);

    private void Awake()
    {
        //inputSystem = new InputSystem();
        rb = GetComponentInChildren<Rigidbody2D>();

        //StateMachine.RegisterState(EPlayerState.Idle, new IdleCharacterState(this));
        //StateMachine.RegisterState(EPlayerState.Walking, new WalkingCharacterState(this));
        //StateMachine.RegisterState(EPlayerState.Interacting, new InteractingCharacterState(this));
        //StateMachine.RegisterState(EPlayerState.Attacking, new AttackingCharacterState(this));

        //StateMachine.SetState(EPlayerState.Idle);
    }

    //private void Update()
    //{
    //    StateMachine.OnUpdate();
    //}

    private void FixedUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        //Direction = inputSystem.Player.Movement.ReadValue<Vector2>(); // Direction salva i valori di movimento presi da input
        /*MoveDirection();*/ // Calcolo del movement applicando accelerazione e decelerazione su assi x e y

        Vector2 test = Direction.normalized;
        //rb.velocity = new Vector2(movement.x, movement.y); // Movimento effettivo
        Test();
        rb.velocity = test * value;
    }

    //private void MoveDirection() // NOTA: aggiunto controllo sul cambio di direzione,
    //                        // per evitare un movimento "scivoloso" quando avviene un cambio di direzione opposto
    //{

    //    if (Mathf.Abs(Direction.x) < 0.01f || changingDirectionX) // Applicazione decelerazione su asse x
    //    {
    //        movement.x = Mathf.MoveTowards(movement.x, 0, deceleration * Time.fixedDeltaTime);
    //    }
    //    else // Applicazione accelerazione su asse x e clamp sulla massima velocità impostata
    //    {
    //        movement.x += Direction.x * acceleration * Time.fixedDeltaTime;
    //        movement.x = Mathf.Clamp(movement.x, -maxSpeed, maxSpeed);
    //    }

    //    if (Mathf.Abs(Direction.y) < 0.01f || changingDirectionY) // Applicazione decelerazione su asse y
    //    {
    //        movement.y = Mathf.MoveTowards(movement.y, 0, deceleration * Time.fixedDeltaTime);
    //    }
    //    else // Applicazione accelerazione su asse y e clamp sulla massima velocità impostata
    //    {
    //        movement.y += Direction.y * acceleration * Time.fixedDeltaTime;
    //        movement.y = Mathf.Clamp(movement.y, -maxSpeed, maxSpeed);
    //    }
    //}

    private void Test()
    {
        if (Direction.magnitude < 0.01f)
        {
            value = Mathf.MoveTowards(value, 0, deceleration * Time.fixedDeltaTime);
        }
        else
        {
            value += acceleration * Time.fixedDeltaTime;
            value = Mathf.Clamp(value, -maxSpeed, maxSpeed);
        }
    }

    public void Attack()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, _damageableMask);
        foreach (Collider2D db in hit)
        {
            Vector2 direction = -(transform.position - db.transform.position).normalized;

            Damageable damageable = db.gameObject.SearchComponent<Damageable>();
            damageable.Damage(_damageAmount, knockback, direction);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, 0.5f);
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
