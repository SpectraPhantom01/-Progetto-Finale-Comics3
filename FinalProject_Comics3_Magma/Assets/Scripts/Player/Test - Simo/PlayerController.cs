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

    [Header("Dashing Settings")]
    public float dashingPower;
    public float dashingTime;
    public float dashingCooldown;

    [Header("Ghost Settings")]
    [SerializeField] GameObject ghostPrefab;
    public float ghostLifeTime;
    public float rewindCooldown;

    [Space(10)]

    [SerializeField] Transform attackPoint;

    //Nota: damageAmount e damageableMask aggiunte per test
    //[Header("Attacking Settings")]
    //[SerializeField] Transform attackPoint;
    //[SerializeField] float knockback = 20f;             // TODO: spostati i valori nel DAMAGER
    //[SerializeField] float _damageAmount; 
    //[SerializeField] LayerMask _damageableMask;

    [HideInInspector] public Vector2 Direction;
    [HideInInspector] public GenericStateMachine<EPlayerState> StateMachine = new GenericStateMachine<EPlayerState>();

    //InputSystem inputSystem;

    [HideInInspector] public Vector2 lastDirection;
    float value;
    //Vector2 normalizedDirection;
    Rigidbody2D rb;
    public Rigidbody2D Rigidbody => rb;

    GameObject instantiatedGhost;
    IEnumerator ghostRoutine;

    //bool changingDirectionX => (rb.velocity.x > 0f && Direction.x < 0f) || (rb.velocity.x < 0f && Direction.x > 0f);
    //bool changingDirectionY => (rb.velocity.y > 0f && Direction.y < 0f) || (rb.velocity.y < 0f && Direction.y > 0f);

    private Damager _damager;

    //public bool IsMoving { get; private set; } = false;
    public bool CanMove { get; set; } = true;
    public bool IsDashing { get; set; } = false;
    public bool CanDash { get; set; } = true;
    public bool GhostActive { get; set; } = false;
    public bool CanRewind { get; set; } = true;    

    private void Awake()
    {
        //inputSystem = new InputSystem();
        rb = GetComponentInChildren<Rigidbody2D>();
        _damager = gameObject.SearchComponent<Damager>();

        CanMove = true;
        IsDashing = false;
        CanDash = true;
        GhostActive = false;
        CanRewind = true;

        //ghostRoutine = GhostRoutine();

        StateMachine.RegisterState(EPlayerState.Idle, new IdleCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Walking, new WalkingCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Interacting, new InteractingCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Attacking, new AttackingCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Dashing, new DashingCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Rewind, new RewindCharacterState(this));

        StateMachine.SetState(EPlayerState.Idle);
    }

    private void Start()
    {
        EquipAttack(EAttackType.Melee);
    }

    private void Update()
    {
        StateMachine.OnUpdate();
    }

    private void FixedUpdate()
    {
        if (CanMove && !IsDashing)
            MoveDirection();
        //Movement();

        StateMachine.OnFixedUpdate();
    }


    
    //Gestione Movimento:
    private void Movement()
    {
        //Direction = inputSystem.Player.Movement.ReadValue<Vector2>(); // Direction salva i valori di movimento presi da input
        /*MoveDirection();*/ // Calcolo del movement applicando accelerazione e decelerazione su assi x e y

        //normalizedDirection = Direction.normalized;
        //rb.velocity = new Vector2(movement.x, movement.y); // Movimento effettivo
        MoveDirection();
        
        //rb.velocity += new Vector2(normalizedDirection.x + value * Time.fixedDeltaTime, normalizedDirection.y + value * Time.fixedDeltaTime);
        //Debug.Log(rb.velocity);
    } //Attualmente obsoleto

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

            rb.velocity = Direction.normalized * value; // Già normalizzata nel GM, da togliere "Direction.normalized"
        }

        AttackPointRotation();
    }



    //Gestione Dash:
    public void Dash()
    {
        if (CanDash && Direction.magnitude > 0) //Controllabile volendo anche da GM
        {
            StateMachine.SetState(EPlayerState.Dashing);
            //StartCoroutine(DashRoutine());
        }
    }

    //public IEnumerator DashRoutine()
    //{
    //    CanDash = false;
    //    IsDashing = true;
    //    rb.velocity = Direction.normalized * dashingPower;

    //    //Debug.Log("Dash effettuato");

    //    yield return new WaitForSeconds(dashingTime);
        
    //    StartCoroutine(DashCooldownRoutine());
    //}

    public IEnumerator DashCooldownRoutine()
    {
        yield return new WaitForSeconds(dashingCooldown);
        CanDash = true;
    }



    //Gestione Rewind:
    public void CreateGhost()
    {
        GhostActive = true;
        instantiatedGhost = Instantiate(ghostPrefab, transform.localPosition, Quaternion.identity);

        ghostRoutine = GhostRoutine(); //Why? A quanto pare serve per la StopCoroutine...funziona così
        StartCoroutine(ghostRoutine);
    }
  
    public void Rewind()
    {
        StopCoroutine(ghostRoutine);
        transform.position = instantiatedGhost.transform.position; 
        DestroyGhost();
    }

    private IEnumerator GhostRoutine()
    {
        yield return new WaitForSeconds(ghostLifeTime);
        DestroyGhost();
    }

    private void DestroyGhost()
    {
        GhostActive = false;
        Destroy(instantiatedGhost);

        CanRewind = false;
        StartCoroutine(RewindCooldownRoutine());
    }

    public IEnumerator RewindCooldownRoutine()
    {
        yield return new WaitForSeconds(rewindCooldown);
        CanRewind = true;
    }



    //Gestione Attacco:
    private void AttackPointRotation()
    {
        Quaternion toRotation = Quaternion.LookRotation(Vector3.back, lastDirection);
        attackPoint.rotation = Quaternion.RotateTowards(attackPoint.rotation, toRotation, 720 * Time.fixedDeltaTime);
    }

    public void Attack() //Da spostare?
    {
        _damager.Attack();
        //_damager.AttackShoot();
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
