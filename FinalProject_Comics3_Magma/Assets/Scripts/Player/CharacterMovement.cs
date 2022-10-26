using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// classe che gestisce il movimento del personaggio prendendo da esterno l'input.
// La chiamata del movimento deve essere gestita dall'esterno, così da poter riutilizzare questa funzione per gli NPC
public class CharacterMovement : MonoBehaviour
{
    [HideInInspector]
    public GenericStateMachine<EPlayerState> StateMachine;

    public Vector2 InputDirection;

    void Start()
    {
        StateMachine.RegisterState(EPlayerState.Idle, new IdleCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Walking, new WalkingCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Jumping, new JumpingCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Falling, new FallingCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Landing, new LandedCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Interacting, new InteractingCharacterState(this));
        StateMachine.RegisterState(EPlayerState.Attacking, new AttackingCharacterState(this));


        StateMachine.SetState(EPlayerState.Walking);
    }

    void Update()
    {
        StateMachine.OnUpdate();
    }

    public void MoveDirection(Vector2 newDirection) // questa funzione deve essere chiamata dall'esterno (per esempio dal PlayerManager che gestisce tutte le altre funzioni del player)
    {
        float x = newDirection.x;
        float y = newDirection.y;

        if (x > 0) // non ricordo perché avessimo fatto questa cosa brutta e cattiva, ma se non ricordo male fu su suggerimento di Piccolino.
            x = 1;
        else if (x < 0)
            x = -1;

        if (y > 0) // idem
            y = 1;
        else if (y < 0)
            y = -1;

        InputDirection = new Vector2(x, y); // questo è il vettore effettivo di movimento che deve essere moltiplicato sul rigidbody per muovere il personaggio ecc.. ecc...
    }                                       // come sempre se trovi altre modalità di gestire la cosa, ben venga! Queste sono solo le funzioni che ho usato finora.


    // OLD CODE FOR FINAL PROJECT 2


    //    namespace MainGame
    //{
    //    public class PlayerMovementManager : Controllable, ISubscriber
    //    {
    //    [HideInInspector]
    //    public GenericStateMachine<EPlayerState> StateMachine;
    //    [HideInInspector]
    //    public Rigidbody2D Rigidbody;
    //    [HideInInspector]
    //    public Collider2D PlayerCollider;
    //    [SerializeField]
    //    public SkeletonAnimation Skeleton;
    //    [SerializeField]
    //    public GameObject GraphicsPivot;
    //    [HideInInspector]
    //    public bool IsGrounded, IsJumping;

    //    [Header("Movement Settings")]
    //    public float InertiaTime;
    //    public float InertiaDecelerator;
    //    public float MaxSpeed;

    //    [Header("Jump Settings")]
    //    public float JumpHeight;
    //    public float JumpDecelerator;
    //    public float TimerJumpButtonIsPressed;
    //    public float GravityScale;
    //    public LayerMask GroundMask;

    //    [Header("Zero Gravity Settings")]
    //    public float SpeedInZeroGravity;

    //    [Header("Interaction Settings")]
    //    [SerializeField] public float InteractionAnimationSpeed;

    //    [Header("Death Settings")]
    //    [SerializeField] private int m_TimesDisplayDeath;
    //    [SerializeField] private float m_TimeAlternateSprite;
    //    [SerializeField] private SpriteRenderer m_DeathSpriteOne;
    //    [SerializeField] private SpriteRenderer m_DeathSpriteTwo;
    //    [SerializeField] AudioSource m_DeathSound;

    //    [Header("Debug Infos"), ReadOnly]
    //    public Vector2 InputDirection;

    //    [Header("Audio Setting")]
    //    public AudioSource stepAudioSource;
    //    public AudioSource JumpAudioSource;
    //    [Space(10)]
    //    public List<AudioClip> listPlayerStep;
    //    public List<AudioClip> listJumpAudio;
    //    public AudioClip landedAudio;


    //    public bool DeadCoroutinePlaying;
    //    private Interacter m_Interacter;
    //    private Vector3 m_NextCheckPoint;
    //    private Damageable m_Damageable;

    //    [HideInInspector]
    //    public EDirection CurrentDirection;
    //    public Vector3 NextCheckpoint => m_NextCheckPoint;
    //    public Interacter Interacter => m_Interacter;
    //    public bool PassingFromZeroG { get; set; }

    //    public bool Flipped { get; private set; } = false;
    //    bool m_VocalZeroG = false;
    //    private void Awake()
    //    {
    //        Rigidbody = GetComponent<Rigidbody2D>();
    //        PlayerCollider = GetComponent<Collider2D>();
    //        StateMachine = new GenericStateMachine<EPlayerState>();
    //        m_Interacter = GetComponent<Interacter>();
    //        m_NextCheckPoint = transform.position;
    //        m_Damageable = GetComponent<Damageable>();
    //    }

    //    void Start()
    //    {
    //        PubSub.PubSub.Subscribe(this, typeof(ZeroGMessage));
    //        PubSub.PubSub.Subscribe(this, typeof(CheckPointMessage));
    //        PubSub.PubSub.Subscribe(this, typeof(PlayerDeathMessage));
    //        PubSub.PubSub.Subscribe(this, typeof(StartEngineModuleMessage));
    //        PubSub.PubSub.Subscribe(this, typeof(DockingCompleteMessage));
    //        PubSub.PubSub.Subscribe(this, typeof(ModuleDestroyedMessage));
    //        PubSub.PubSub.Subscribe(this, typeof(NoBatteryMessage));
    //        PubSub.PubSub.Subscribe(this, typeof(ActivationVocalZeroG));

    //        PubSub.PubSub.Subscribe(this, typeof(StartDialogueMessage));


    //        Skeleton.state.Event += OnEvent;


    //        StateMachine.RegisterState(EPlayerState.Walking, new WalkingPlayerState(this));
    //        StateMachine.RegisterState(EPlayerState.Jumping, new JumpingPlayerState(this));
    //        StateMachine.RegisterState(EPlayerState.ZeroG, new ZeroGPlayerState(this));
    //        StateMachine.RegisterState(EPlayerState.Landing, new LandingPlayerState(this));
    //        StateMachine.RegisterState(EPlayerState.Flying, new FlyingPlayerState(this));
    //        StateMachine.RegisterState(EPlayerState.Somersault, new SomersaultPlayerState(this));
    //        StateMachine.RegisterState(EPlayerState.Landed, new LandedPlayerState(this));
    //        StateMachine.RegisterState(EPlayerState.Interacting, new InteractingPlayerState(this));

    //        StateMachine.SetState(EPlayerState.Walking);
    //    }

    //    private void OnEvent(TrackEntry trackEntry, Spine.Event e)
    //    {
    //        if (e.Data.Name == "Passi Camminata")
    //        {
    //            stepAudioSource.PlayOneShot(AudioManager.GetRandomAudioClip(listPlayerStep));
    //        }
    //        else if (e.Data.Name == "Salto Sforzo" || e.Data.Name == "GravitàApice")
    //        {
    //            JumpAudioSource.PlayOneShot(AudioManager.GetRandomAudioClip(listJumpAudio));
    //        }
    //    }

    //    void Update()
    //    {

    //        StateMachine.OnUpdate();
    //        GroundCheck();
    //    }

    //    private void FixedUpdate()
    //    {
    //        StateMachine.OnFixedUpdate();
    //    }

    //    private void OnCollisionEnter2D(Collision2D collision)
    //    {
    //        StateMachine.MyOnCollisionEnter2D(collision);
    //    }

    //    private void OnTriggerEnter2D(Collider2D collision)
    //    {
    //        JumpIncreaser ji = collision.GetComponentInParent<JumpIncreaser>();

    //        if (collision.GetComponentInParent<Lift>() != null)
    //        {
    //            if (StateMachine.CurrentState.GetType() != typeof(FlyingPlayerState))
    //            {
    //                if (StateMachine.CurrentState is ZeroGPlayerState)
    //                    PubSub.PubSub.Publish(new ZeroGMessage(false));

    //                StateMachine.SetState(EPlayerState.Flying);
    //            }
    //        }
    //        else if (ji != null)
    //        {
    //            ji.StoreJumpHegiht(JumpHeight);
    //            JumpHeight = ji.GetNewJumpHeght();
    //        }
    //    }

    //    private void OnTriggerExit2D(Collider2D collision)
    //    {
    //        JumpIncreaser ji = collision.GetComponentInParent<JumpIncreaser>();

    //        if (collision.GetComponentInParent<Lift>() != null)
    //        {
    //            StateMachine.SetState(EPlayerState.Walking);
    //        }
    //        else if (ji != null)
    //        {
    //            JumpHeight = ji.GetOldJumpHeght();
    //        }
    //    }

    //    private void GroundCheck()
    //    {
    //        if (!ForwardCheckOfWall(Vector3.down, 0.01f))
    //        {
    //            IsGrounded = true;
    //        }
    //        else
    //        {
    //            IsGrounded = false;
    //        }
    //    }

    //    public void Movement()
    //    {

    //        if (InputDirection.x > 0.001f && Flipped)
    //        {
    //            FlipSpriteOnX(false);
    //        }
    //        if (InputDirection.x < -0.001f && !Flipped)
    //        {
    //            FlipSpriteOnX(true);
    //        }

    //        if (!DeadCoroutinePlaying)
    //            Rigidbody.velocity = InputDirection * MaxSpeed * Time.fixedDeltaTime;


    //    }

    //    public override void MoveDirection(Vector2 newDirection)
    //    {
    //        float x = newDirection.x;
    //        float y = newDirection.y;

    //        if (x > 0)
    //            x = 1;
    //        else if (x < 0)
    //            x = -1;

    //        if (y > 0)
    //            y = 1;
    //        else if (y < 0)
    //            y = -1;

    //        InputDirection = new Vector2(x, y);
    //    }

    //    public override void Jump(bool jumping)
    //    {
    //        IsJumping = jumping;
    //    }

    //    public void FlipSpriteOnX(bool flipped)
    //    {
    //        Vector3 scale = flipped ? Vector3.back : Vector3.forward;
    //        Skeleton.gameObject.transform.rotation = Quaternion.LookRotation(scale, Vector3.up);
    //        Flipped = flipped;
    //    }


    //    public void OnPublish(IMessage message)
    //    {
    //        if (message is ZeroGMessage)
    //        {
    //            ZeroGMessage zeroGMessage = (ZeroGMessage)message;
    //            ContinousMovement = zeroGMessage.Active;
    //            CurrentDirection = EDirection.Up;
    //            if (zeroGMessage.Active)
    //                StateMachine.SetState(EPlayerState.ZeroG);
    //            else
    //                StateMachine.SetState(EPlayerState.Walking);
    //        }
    //        else if (message is CheckPointMessage)
    //        {
    //            CheckPointMessage checkPoint = (CheckPointMessage)message;
    //            m_NextCheckPoint = checkPoint.Position;
    //        }
    //        else if (message is PlayerDeathMessage)
    //        {
    //            StartCoroutine(PlayerDeathMessage());
    //        }
    //        else if (message is StartEngineModuleMessage)
    //        {
    //            PlayerCollider.enabled = false;
    //        }
    //        else if (message is DockingCompleteMessage || message is NoBatteryMessage || message is ModuleDestroyedMessage)
    //        {
    //            PlayerCollider.enabled = true;
    //        }
    //        else if (message is StartDialogueMessage)
    //        {
    //            InputDirection = Vector2.zero;
    //        }
    //        else if (message is ActivationVocalZeroG)
    //        {
    //            m_VocalZeroG = true;
    //        }
    //    }

    //    private IEnumerator PlayerDeathMessage()
    //    {
    //        m_DeathSound.Play();
    //        Skeleton.gameObject.SetActive(false);
    //        Rigidbody.velocity = Vector2.zero;
    //        Rigidbody.bodyType = RigidbodyType2D.Static;
    //        DeadCoroutinePlaying = true;
    //        PlayerCollider.enabled = false;

    //        int counter = 0;
    //        while (counter < m_TimesDisplayDeath)
    //        {

    //            m_DeathSpriteOne.enabled = true;
    //            m_DeathSpriteTwo.enabled = false;

    //            yield return new WaitForSeconds(m_TimeAlternateSprite);

    //            m_DeathSpriteOne.enabled = false;
    //            m_DeathSpriteTwo.enabled = true;

    //            yield return new WaitForSeconds(m_TimeAlternateSprite);

    //            counter++;
    //        }

    //        Skeleton.gameObject.SetActive(true);
    //        Rigidbody.bodyType = RigidbodyType2D.Dynamic;
    //        RespawnPlayer();
    //        m_DeathSpriteOne.enabled = false;
    //        m_DeathSpriteTwo.enabled = false;
    //        DeadCoroutinePlaying = false;
    //        PlayerCollider.enabled = true;

    //    }

    //    public void RespawnPlayer()
    //    {
    //        transform.position = m_NextCheckPoint;

    //        if (StateMachine.CurrentState.GetType() == typeof(ZeroGPlayerState))
    //        {
    //            ZeroGPlayerState zeroGPlayerState = (ZeroGPlayerState)StateMachine.GetState(EPlayerState.ZeroG);
    //            zeroGPlayerState.StopMovement();
    //        }
    //        m_Damageable.SetInitialLife(1);
    //    }

    //    public void OnDisableSubscribe()
    //    {
    //        PubSub.PubSub.Unsubscribe(this, typeof(ZeroGMessage));
    //        PubSub.PubSub.Unsubscribe(this, typeof(CheckPointMessage));
    //        PubSub.PubSub.Unsubscribe(this, typeof(PlayerDeathMessage));
    //        PubSub.PubSub.Unsubscribe(this, typeof(StartEngineModuleMessage));
    //        PubSub.PubSub.Unsubscribe(this, typeof(DockingCompleteMessage));
    //        PubSub.PubSub.Unsubscribe(this, typeof(NoBatteryMessage));
    //        PubSub.PubSub.Unsubscribe(this, typeof(ModuleDestroyedMessage));
    //        PubSub.PubSub.Unsubscribe(this, typeof(ActivationVocalZeroG));

    //        PubSub.PubSub.Unsubscribe(this, typeof(StartDialogueMessage));
    //    }

    //    private void OnDestroy()
    //    {
    //        OnDisableSubscribe();
    //    }

    //    public override void Interact()
    //    {

    //        if (Rigidbody.velocity.magnitude == 0 && Interacter.InteractionAvailable && StateMachine.CurrentState.GetType() != typeof(InteractingPlayerState) && StateMachine.CurrentState.GetType() != typeof(SomersaultPlayerState))
    //        {
    //            if (StateMachine.CurrentState.GetType() == typeof(ZeroGPlayerState))
    //            {
    //                PassingFromZeroG = true;
    //                ZeroGPlayerState zeroGPlayerState = (ZeroGPlayerState)StateMachine.GetState(EPlayerState.ZeroG);
    //                zeroGPlayerState.PassedThroughInteraction = true;
    //            }
    //            StateMachine.SetState(EPlayerState.Interacting);
    //        }
    //    }

    //    public void InteractableAvailableCheck()
    //    {
    //        m_Interacter.RemoveInteractableCheck();
    //    }

    //    public void RotatePlayer(int degrees, Vector3 adjusterPos)
    //    {
    //        GraphicsPivot.transform.eulerAngles = new Vector3(0, 0, degrees);
    //        GraphicsPivot.transform.localPosition += adjusterPos;
    //    }

    //    public override void VocalZeroG()
    //    {
    //        if (m_VocalZeroG)
    //        {
    //            if (StateMachine.CurrentState is ZeroGPlayerState)
    //                PubSub.PubSub.Publish(new ZeroGMessage(false));
    //            else
    //                PubSub.PubSub.Publish(new ZeroGMessage(true));
    //        }
    //    }


    //}
    //}
}
