using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
    [SerializeField] float ghostLifeTime;
    public float rewindCooldown;
    [Tooltip("This prefab will follow the player but won't do any damage")]
    [SerializeField] PlayerController playerGhostPrefab;

    [Space(10)]
    [SerializeField] Transform attackPoint;
    public Transform AttackPosition;

    [Header("SFX")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip activeGhost;
    [SerializeField] AudioClip destroyGhost;
    [SerializeField] List<AudioClip> attackAudioList;
    [SerializeField] List<AudioClip> dashAudioList;
    [SerializeField] List<AudioClip> stepAudioList;
    [SerializeField] List<AudioClip> hitAudioList;
    [SerializeField] List<AudioClip> hitEnemyAudioList;
    [SerializeField] float timeBetweenSteps = 0.5f;

    [HideInInspector] public Vector2 Direction;
    [HideInInspector] public GenericStateMachine<EPlayerState> StateMachine = new GenericStateMachine<EPlayerState>();
    [HideInInspector] public Damager Damager;
    [HideInInspector] public Vector2 lastDirection;
    [HideInInspector] public float WalkingSoundTime = 0;
    [HideInInspector] public UnityEvent Listeners;
    float value;
    Rigidbody2D rb;
    PlayerController instantiatedGhost;
    Coroutine ghostRoutine;
    List<Vector2> ghostPositions = new List<Vector2>();

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
    public bool FakeGhost { get; set; } = false;
    public List<Vector2> GhostPositions { get => ghostPositions; }

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
        ResetWalkingTimeSound();
        if (!ImGhost)
        {
            _playerManager.Damageable.onGetDamage += TryDestroyGhost;
            _playerManager.Damageable.onGetDamage += () => PlayRandomSoundOnList(hitAudioList);
        }


    }

    private void TryDestroyGhost()
    {
        if (GhostActive)
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

    public void Initialize(bool isGhost, PlayerManager playerManager, bool fakeGhost)
    {
        ImGhost = isGhost;
        Father = playerManager;
        FakeGhost = fakeGhost;
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
            value += acceleration * Time.fixedDeltaTime;

            var equipments = GetCurrentEquipment(ImGhost ? Father.Inventory : _playerManager?.Inventory);

            var bonusSpeed1 = equipments[0] != null ? equipments[0].PickableEffectType == EPickableEffectType.AddMovementSpeed ? equipments[0].EffectInTime : 0 : 0;
            var bonusSpeed2 = equipments[1] != null ? equipments[1].PickableEffectType == EPickableEffectType.AddMovementSpeed ? equipments[1].EffectInTime : 0 : 0;

            var speed = maxSpeed + bonusSpeed1 + bonusSpeed2;

            value = Mathf.Clamp(value, -speed, speed);

            rb.velocity = Direction * value;
        }

        AttackPointRotation();
    }



    //Gestione Dash:
    public void Dash()
    {
        if (CanDash && Direction.magnitude > 0)
        {
            PlayRandomSoundOnList(dashAudioList);
            StateMachine.SetState(EPlayerState.Dashing);
        }
    }

    private void PlayRandomSoundOnList(List<AudioClip> audioClipList)
    {
        if (!ImGhost)
            audioSource.PlayOneShot(audioClipList[UnityEngine.Random.Range(0, audioClipList.Count)]);
    }

    public IEnumerator DashCooldownRoutine()
    {
        yield return new WaitForSeconds(dashingCooldown);
        CanDash = true;
    }



    //Gestione Rewind:
    public void CreateGhost()
    {
        GhostActive = true;

        audioSource.PlayOneShot(activeGhost);

        if (ghostPositions.Count > 0)
        {
            for (int i = 0; i < ghostPositions.Count; i++)
            {
                if (i == ghostPositions.Count - 1)
                    InstantiateGhost(ghostPositions[i], i);
                else
                    InstantiateGhost(ghostPositions[i], 0);
            }
        }
        else
        {
            InstantiateGhost(transform.localPosition, 0);
        }

        ghostRoutine = StartCoroutine(GhostRoutine(GetGhostLifeTime()));

        GameManager.Instance.GhostManager.StartReadingDistance();
    }

    public float GetGhostLifeTime()
    {
        var equipments = GetCurrentEquipment(ImGhost ? Father.Inventory : _playerManager?.Inventory);
        var bonusGhostTime1 = equipments[0] != null ? equipments[0].PickableEffectType == EPickableEffectType.AddGhostTime ? equipments[0].EffectInTime : 0 : 0;
        var bonusGhostTime2 = equipments[1] != null ? equipments[1].PickableEffectType == EPickableEffectType.AddGhostTime ? equipments[1].EffectInTime : 0 : 0;

        return ghostLifeTime + bonusGhostTime1 + bonusGhostTime2;

    }

    private void InstantiateGhost(Vector2 position, int index)
    {
        instantiatedGhost = Instantiate(ghostPrefab, position, Quaternion.Euler(-90, 0, 0));
        instantiatedGhost.Initialize(true, PlayerManager, false);

        instantiatedGhost.PlayerManager.CurrentDirection = PlayerManager.CurrentDirection;
        instantiatedGhost.lastDirection = lastDirection;

        if (index == 0)
        {
            var playerGhost = Instantiate(playerGhostPrefab, position, Quaternion.Euler(-90, 0, 0));
            playerGhost.Initialize(true, PlayerManager, true);

            playerGhost.PlayerManager.CurrentDirection = PlayerManager.CurrentDirection;
            playerGhost.lastDirection = lastDirection;

            GameManager.Instance.PlayerGhostControllerList.Add(playerGhost);
        }

        GameManager.Instance.GhostManager.AddGhost(instantiatedGhost);
    }

    public void Rewind()
    {
        StopCoroutine(ghostRoutine);
        transform.position = instantiatedGhost.transform.position;
        audioSource.PlayOneShot(activeGhost);

        GhostActivation();

        Listeners?.Invoke();

        Listeners.RemoveAllListeners();
    }

    private IEnumerator GhostRoutine(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyGhost();

        Listeners?.Invoke();

        Listeners.RemoveAllListeners();

        audioSource.PlayOneShot(destroyGhost);
    }

    public void DestroyGhost()
    {
        CanRewind = false;
        GhostActive = false;


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

        if (!ImGhost)
        {
            PlayRandomSoundOnList(attackAudioList);

            if (!TryPickUp())
            {
                StateMachine.SetState(EPlayerState.Attacking);

                //da fare uno switch per sapere se c'è bisogno di attaccare oppure raccogliere l'oggetto.
                var equipments = GetCurrentEquipment(ImGhost ? Father.Inventory : _playerManager?.Inventory);
                float bonusAttack1 = equipments[0] != null ? equipments[0].PickableEffectType == EPickableEffectType.AddAttackForce ? equipments[0].EffectInPercentage : 0 : 0;
                float bonusAttack2 = equipments[1] != null ? equipments[1].PickableEffectType == EPickableEffectType.AddAttackForce ? equipments[1].EffectInPercentage : 0 : 0;

                Damager.SetBonusAttack(bonusAttack1, bonusAttack2);

                StartCoroutine(AttackCoroutine());
            }
        }
        else if (!FakeGhost)
        {
            StateMachine.SetState(EPlayerState.Attacking);

            StartCoroutine(AttackCoroutine());
        }
    }

    public IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        if (!Damager.SearchInteractable())
            if (Damager.Attack())
            {
                PlayRandomSoundOnList(hitEnemyAudioList);
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
        if (!FakeGhost)
            Damager.EquipAttack(eAttackType);
    }

    public bool TryPickUp()
    {
        var collidersHit = Physics2D.OverlapCircleAll(transform.position, 1).ToList();
        if (collidersHit.Count > 0)
        {
            foreach (var hit in collidersHit)
            {
                if (hit.TryGetComponent<PickableObject>(out var pickable))
                {
                    _playerManager.PickUpObject(pickable.PickableScriptableObject, pickable);
                    return true;
                }
            }
        }
        return false;
    }

    public void UpdateWalkingSound()
    {
        WalkingSoundTime += Time.deltaTime;
        if (WalkingSoundTime >= timeBetweenSteps)
        {
            WalkingSoundTime = 0;
            PlayRandomSoundOnList(stepAudioList);
        }
    }

    public void ResetWalkingTimeSound()
    {
        WalkingSoundTime = timeBetweenSteps;
    }

}
