using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movemens Settings")]
    [Tooltip("Velocita massima del player")]
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
    [SerializeField] PlayerController ghostPrefab;
    public float ghostLifeTime;
    public float rewindCooldown;

    [Space(10)]
    [SerializeField] Transform attackPoint;
    public Transform AttackPosition;


    [HideInInspector] public Vector2 Direction;
    [HideInInspector] public GenericStateMachine<EPlayerState> StateMachine = new GenericStateMachine<EPlayerState>();
    [HideInInspector] public Damager Damager;
    [HideInInspector] public Vector2 lastDirection;

    float value;
    Rigidbody2D rb;
    PlayerController instantiatedGhost;
    Coroutine ghostRoutine;


    //public bool IsMoving { get; private set; } = false;
    public Rigidbody2D Rigidbody => rb;
    public bool CanMove { get; set; } = true;
    public bool IsDashing { get; set; } = false;
    public bool CanDash { get; set; } = true;
    public bool GhostActive { get; set; } = false;
    public bool CanRewind { get; set; } = true;
    public bool IsAttacking { get; set; } = false;
    public bool IsMoving => value != 0;
    private PlayerManager _playerManager;
    public PlayerManager PlayerManager => _playerManager;
    public PlayerManager Father;
    public bool ImGhost { get; set; } = false;

    private void Awake()
    {
        Damager = gameObject.SearchComponent<Damager>();
        
        rb = GetComponent<Rigidbody2D>();
        _playerManager = GetComponent<PlayerManager>();
        CanMove = true;
        IsDashing = false;
        CanDash = true;
        GhostActive = false;
        CanRewind = true;

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
        if (!ImGhost)
        {
            _playerManager.Damageable.onGetDamage += TryDestroyGhost;
        }
    }

    private void TryDestroyGhost()
    {
        if(GhostActive)
        {
            InterruptGhost();
        }
    }

    private void Update()
    {
        StateMachine.OnUpdate();
    }

    private void FixedUpdate()
    {
        if (CanMove && !IsDashing)
            MoveDirection();

        StateMachine.OnFixedUpdate();
    }

    public void Initialize(bool isGhost, PlayerManager playerManager)
    {
        ImGhost = isGhost;
        Father = playerManager;
    }

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

            var equipments = GetCurrentEquipment(ImGhost ? Father.Inventory : _playerManager?.Inventory);

            var bonusSpeed1 = equipments[0] != null ? equipments[0].PickableEffectType == EPickableEffectType.AddMovementSpeed ? equipments[0].EffectInTime : 0 : 0;
            var bonusSpeed2 = equipments[1] != null ? equipments[1].PickableEffectType == EPickableEffectType.AddMovementSpeed ? equipments[1].EffectInTime : 0 : 0;

            var speed = maxSpeed + bonusSpeed1 + bonusSpeed2;

            value = Mathf.Clamp(value, -speed, speed);

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

        var equipments = GetCurrentEquipment(ImGhost ? Father.Inventory : _playerManager?.Inventory);
        var bonusGhostTime1 = equipments[0] != null ? equipments[0].PickableEffectType == EPickableEffectType.AddGhostTime ? equipments[0].EffectInTime : 0 : 0;
        var bonusGhostTime2 = equipments[1] != null ? equipments[1].PickableEffectType == EPickableEffectType.AddGhostTime ? equipments[1].EffectInTime : 0 : 0;

        var ghostTime = ghostLifeTime + bonusGhostTime1 + bonusGhostTime2;

        instantiatedGhost = Instantiate(ghostPrefab, transform.localPosition, Quaternion.Euler(-90, 0, 0));
        instantiatedGhost.Initialize(true, PlayerManager);
        //ghostRoutine = GhostRoutine(); //Why? A quanto pare serve per la StopCoroutine...funziona così
        //StartCoroutine(ghostRoutine);
        ghostRoutine = StartCoroutine(GhostRoutine(ghostTime)); // ghostRoutine è una classe Coroutine, non IEnumerator, così la puoi gestire in questo modo

        GameManager.Instance.GhostManager.StartReadingDistance(instantiatedGhost);
    }
  
    public void Rewind()
    {
        StopCoroutine(ghostRoutine);
        transform.position = instantiatedGhost.transform.position;
        //DestroyGhost();
        GhostActivation();
    }

    private IEnumerator GhostRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyGhost();
    }

    public void DestroyGhost()
    {
        CanRewind = false;
        GhostActive = false;

        //Destroy(instantiatedGhost);
        //instantiatedGhost = null;

        GameManager.Instance.GhostManager.ResetGhost();

        StartCoroutine(RewindCooldownRoutine());
    }

    public void InterruptGhost()
    {
        StopCoroutine(ghostRoutine);
        DestroyGhost();
    }

    private void GhostActivation()
    {
        CanRewind = false;
        GhostActive = false;  
        
        GameManager.Instance.GhostManager.StartExecuteInputs();
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

    public void Attack() 
    {
        if(!ImGhost)
        {
            if (!TryPickUp())
            {
                StateMachine.SetState(EPlayerState.Attacking);

                //da fare uno switch per sapere se c'è bisogno di attaccare oppure raccogliere l'oggetto.
                var equipments = GetCurrentEquipment(ImGhost ? Father.Inventory : _playerManager?.Inventory);
                float bonusAttack1 = equipments[0] != null ? equipments[0].PickableEffectType == EPickableEffectType.AddAttackForce ? equipments[0].EffectInPercentage : 0 : 0;
                float bonusAttack2 = equipments[1] != null ? equipments[1].PickableEffectType == EPickableEffectType.AddAttackForce ? equipments[1].EffectInPercentage : 0 : 0;

                Damager.SetBonusAttack(bonusAttack1, bonusAttack2);

                Damager.Attack(); // <<---
                Damager.SearchInteractable();
            }
        }
        else
        {
            StateMachine.SetState(EPlayerState.Attacking);
            Damager.Attack();
            Damager.SearchInteractable();
        }
    }

    private PickableScriptableObject[] GetCurrentEquipment(Inventory inventory)
    {
        PickableScriptableObject[] pickableScriptableObjects = new PickableScriptableObject[2];
        pickableScriptableObjects[0] = inventory?.EquipmentSlots[0]?.PickableSO;
        pickableScriptableObjects[1] = inventory?.EquipmentSlots[1]?.PickableSO;
        return pickableScriptableObjects;
    }

    public void EquipAttack(EAttackType eAttackType)
    {
        Damager.EquipAttack(eAttackType);
    }

    public bool TryPickUp()
    {
        var collidersHit = Physics2D.OverlapCircleAll(transform.position, 1).ToList();
        if (collidersHit.Count > 0)
        {
            foreach (var hit in collidersHit)
            {
                if(hit.TryGetComponent<PickableObject>(out var pickable))
                {
                    _playerManager.PickUpObject(pickable.PickableScriptableObject, pickable.gameObject);
                    return true;
                }
            }
        }
        return false;
    }

}
