using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movemens Settings")]
    [Tooltip("Velocit� massima del player")]
    [SerializeField] float maxSpeed = 6.5f;
    [Tooltip("Accelerazione del player")]
    [SerializeField] float acceleration = 25;
    [Tooltip("Decelerazione del player")]
    [SerializeField] float deceleration = 35;

    [SerializeField] Transform attackPoint;

    //Nota: damageAmount e damageableMask aggiunte per test
    //[Header("Attacking Settings")]
    //[SerializeField] Transform attackPoint;
    //[SerializeField] float knockback = 20f;             // TODO: spostati i valori nel DAMAGER
    //[SerializeField] float _damageAmount; 
    //[SerializeField] LayerMask _damageableMask;

    [HideInInspector] public Vector2 Direction;
    //[HideInInspector] public GenericStateMachine<EPlayerState> StateMachine;

    //InputSystem inputSystem;

    [HideInInspector] public Vector2 lastDirection;
    float value;
    //Vector2 normalizedDirection;
    Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    //bool changingDirectionX => (rb.velocity.x > 0f && Direction.x < 0f) || (rb.velocity.x < 0f && Direction.x > 0f);
    //bool changingDirectionY => (rb.velocity.y > 0f && Direction.y < 0f) || (rb.velocity.y < 0f && Direction.y > 0f);

    private Damager _damager;

    //public bool IsMoving { get; private set; } = false;
    public bool CanMove { get; set; } = true;
    public bool IsDashing { get; set; } = false;
    public bool CanDash { get; set; } = true;

    private void Awake()
    {
        //inputSystem = new InputSystem();
        rb = GetComponentInChildren<Rigidbody2D>();
        _damager = gameObject.SearchComponent<Damager>();

        CanMove = true;
        IsDashing = false;
        CanDash = true;

        //StateMachine.RegisterState(EPlayerState.Idle, new IdleCharacterState(this));
        //StateMachine.RegisterState(EPlayerState.Walking, new WalkingCharacterState(this));
        //StateMachine.RegisterState(EPlayerState.Interacting, new InteractingCharacterState(this));
        //StateMachine.RegisterState(EPlayerState.Attacking, new AttackingCharacterState(this));

        //StateMachine.SetState(EPlayerState.Idle);
    }

    private void Start()
    {
        EquipAttack(EAttackType.Melee);
    }

    //private void Update()
    //{
    //    StateMachine.OnUpdate();
    //}

    private void FixedUpdate()
    {
        if (CanMove && !IsDashing)
            Movement();
    }

    private void Movement()
    {
        //Direction = inputSystem.Player.Movement.ReadValue<Vector2>(); // Direction salva i valori di movimento presi da input
        /*MoveDirection();*/ // Calcolo del movement applicando accelerazione e decelerazione su assi x e y

        //normalizedDirection = Direction.normalized;
        //rb.velocity = new Vector2(movement.x, movement.y); // Movimento effettivo
        MoveDirection();
        
        //rb.velocity += new Vector2(normalizedDirection.x + value * Time.fixedDeltaTime, normalizedDirection.y + value * Time.fixedDeltaTime);
        //Debug.Log(rb.velocity);
    }

    //private void MoveDirection() // NOTA: aggiunto controllo sul cambio di direzione,
    //                        // per evitare un movimento "scivoloso" quando avviene un cambio di direzione opposto
    //{

    //    if (Mathf.Abs(Direction.x) < 0.01f || changingDirectionX) // Applicazione decelerazione su asse x
    //    {
    //        movement.x = Mathf.MoveTowards(movement.x, 0, deceleration * Time.fixedDeltaTime);
    //    }
    //    else // Applicazione accelerazione su asse x e clamp sulla massima velocit� impostata
    //    {
    //        movement.x += Direction.x * acceleration * Time.fixedDeltaTime;
    //        movement.x = Mathf.Clamp(movement.x, -maxSpeed, maxSpeed);
    //    }

    //    if (Mathf.Abs(Direction.y) < 0.01f || changingDirectionY) // Applicazione decelerazione su asse y
    //    {
    //        movement.y = Mathf.MoveTowards(movement.y, 0, deceleration * Time.fixedDeltaTime);
    //    }
    //    else // Applicazione accelerazione su asse y e clamp sulla massima velocit� impostata
    //    {
    //        movement.y += Direction.y * acceleration * Time.fixedDeltaTime;
    //        movement.y = Mathf.Clamp(movement.y, -maxSpeed, maxSpeed);
    //    }
    //}

    private void MoveDirection()
    {
        if (Direction.magnitude < 0.01f)
        {
            value = Mathf.MoveTowards(value, 0, deceleration * Time.fixedDeltaTime);

            rb.velocity = lastDirection * value;
        }
        else
        {
            //lastDirection = normalizedDirection;

            value += acceleration * Time.fixedDeltaTime;
            value = Mathf.Clamp(value, -maxSpeed, maxSpeed);

            rb.velocity = Direction.normalized * value;
        }

        AttackPointRotation();
    }

    private void AttackPointRotation()
    {
        Quaternion toRotation = Quaternion.LookRotation(Vector3.back, lastDirection); 
        attackPoint.rotation = Quaternion.RotateTowards(attackPoint.rotation, toRotation, 720 * Time.fixedDeltaTime);
    }

    public void Attack()
    {
        //Collider2D[] hit = Physics2D.OverlapCircleAll(attackPoint.position, 0.5f, _damageableMask);
        //foreach (Collider2D db in hit)
        //{
        //    Vector2 direction = -(transform.position - db.transform.position).normalized;

        //    Damageable damageable = db.gameObject.SearchComponent<Damageable>();
        //    if(damageable != null)
        //        damageable.Damage(_damageAmount, knockback, direction);
        //}

        _damager.Attack();
        //_damager.AttackShoot();
    }

    public void Roll()
    {
        if(CanDash)
            StartCoroutine(RollRoutine());
    }

    private IEnumerator RollRoutine()
    {
        CanDash = false;
        IsDashing = true;
        rb.velocity = Direction.normalized * 20f;

        yield return new WaitForSeconds(1f);
        IsDashing = false;

        yield return new WaitForSeconds(3f);
        CanDash = true;
    }
    public void EquipAttack(EAttackType eAttackType)
    {
        _damager.EquipAttack(eAttackType);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(attackPoint.position, 0.5f);
    //}

    //private void OnEnable()
    //{
    //    inputSystem.Player.Enable();
    //}

    //private void OnDisable()
    //{
    //    inputSystem.Player.Disable();
    //}
}
