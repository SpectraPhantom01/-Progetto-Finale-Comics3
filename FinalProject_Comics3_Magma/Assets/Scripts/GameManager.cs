using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour, ISubscriber
{
    #region SINGLETON
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance != null)
                    return instance;

                GameObject go = new GameObject("GameManager");
                return go.AddComponent<GameManager>();
            }
            else
                return instance;
        }
        set
        {
            instance = value;
        }
    }
    #endregion

    [Header("Player Settings")]
    [SerializeField] PlayerManager PlayerPrefab;

    [Header("Enemy Settings")]
    [SerializeField] EnemyController EnemyPrefab;

    [SerializeField] Volume globalVolume;
    // Corrected Variables
    private InputSystem inputSystem;
    private PlayerController playerController;
    private GhostManager ghostManager;
    private LanguageManager languageManager;

    // Properties
    public PlayerController Player => playerController;

    public GhostManager GhostManager => ghostManager;
    Coroutine ghostEffect;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);


        playerController = FindObjectOfType<PlayerController>();
        ghostManager = GetComponent<GhostManager>();
        ghostManager.Initialize(playerController);
        //if (playerMovement == null)
        //    throw new Exception("PlayerMovement assente nella scena attuale, importare il prefab del player!!!");

        // INPUT SYSTEM

        inputSystem = new InputSystem();
        inputSystem.Player.Enable();
        inputSystem.Player.Movement.performed += Movement_started;
        inputSystem.Player.Movement.canceled += Movement_canceled;
        inputSystem.Player.Attack.performed += Attack_performed;
        inputSystem.Player.Dash.performed += Dash_performed;
        inputSystem.Player.Rewind.performed += Rewind_performed;
        inputSystem.Player.Pause.performed += Pause_performed;
        inputSystem.Player.ActiveObjectOne.performed += ActiveObjectOnePerformed;
        inputSystem.Player.ActiveObjectTwo.performed += ActiveObjectTwoPerformed;
        inputSystem.Player.ActiveObjectThree.performed += ActiveObjectThreePerformed;
        inputSystem.Player.ActiveObjectFour.performed += ActiveObjectFourPerformed;
        // END INPUT SYSTEM



        if (!FileSystem.Load("LanguageManager", "csv", out string[] fileLoaded))
            throw new Exception("File LanguageManager non caricato correttamente, controllare eventuali posizioni del file o il nome del file o l'estensione del file");

        CSVFileReader.Parse(fileLoaded, ';', "", true);

        languageManager = new LanguageManager(CSVFileReader.FileMatrix);

        Publisher.Subscribe(this, typeof(SaveMessage));
        Publisher.Subscribe(this, typeof(LoadMessage));
    }

    private void ActiveObjectOnePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.PlayerManager.TryUseObject(0);
    }
    private void ActiveObjectTwoPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.PlayerManager.TryUseObject(1);
    }
    private void ActiveObjectThreePerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.PlayerManager.TryUseObject(2);
    }
    private void ActiveObjectFourPerformed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.PlayerManager.TryUseObject(3);
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UIManager.Instance.Pause();
    }

    private void Rewind_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (playerController.CanRewind)
        {
            if (playerController.GhostActive)
            {
                // finish ghost, destroy object
                GhostManager.RegistraInput(Vector2.zero, InputType.Ghost);

            }

            playerController.StateMachine.SetState(EPlayerState.Rewind);
            if (playerController.Direction.magnitude != 0)
            {
                // if he is already moving, register a state
                GhostManager.RegistraInput(playerController.Direction, InputType.MovementStart);

            }
        }

        EnableGhostRendering();
    }

    public void EnableGhostRendering()
    {
        if (playerController.GhostActive)
        {

            globalVolume.gameObject.SetActive(true);
            ghostEffect = StartCoroutine(GhostEffect());
        }
        else
        {

            globalVolume.gameObject.SetActive(false);
            StopCoroutine(ghostEffect);
        }
    }

    private IEnumerator GhostEffect()
    {
        globalVolume.profile.TryGet<ChromaticAberration>(out var type);
        float intensity = 0.5f;
        bool goingUp = true;
        bool goingDown = false;
        while (true)
        {
            type.intensity.Override(intensity);
            yield return null;

            if(goingDown)
            { 
                intensity -= 0.01f;
                if (intensity <= 0.5f)
                {
                    goingUp = true;
                    goingDown = false;
                }
            }
            else if(goingUp)
            {
                intensity += 0.01f;
                if(intensity >= 1)
                {
                    goingUp = false;
                    goingDown = true;
                }
            }
        }
    }

    private void Dash_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (playerController.GhostActive)
        {
            GhostManager.RegistraInput(Vector2.zero, InputType.Dash);
        }

        playerController.Dash();

        //playerController.StateMachine.SetState(EPlayerState.Dashing);
    }

    private void Movement_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (playerController.GhostActive)
        {
            GhostManager.RegistraInput(obj.ReadValue<Vector2>(), InputType.MovementEnd);
        }

        playerController.Direction = obj.ReadValue<Vector2>();
    }

    private void Movement_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Vector2 readedDirection = obj.ReadValue<Vector2>();

        playerController.Direction = readedDirection.normalized;
        playerController.lastDirection = readedDirection.normalized;

        if (playerController.GhostActive)
        {
            GhostManager.RegistraInput(obj.ReadValue<Vector2>(), InputType.MovementStart);
        }

        if (/*!playerController.IsDashing */ playerController.CanMove)
        {
            playerController.StateMachine.SetState(EPlayerState.Walking);           
        }      
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (playerController.GhostActive)
        {
            GhostManager.RegistraInput(Vector2.zero, InputType.Attack);
        }

        if(!playerController.IsAttacking) 
            playerController.Attack();
    }

    private void Start()
    {
        //Invoke("ChangeLanguage", 5);
    }
    private void ChangeLanguage()
    {
        Publisher.Publish(new ChangeLanguageMessage(ELanguage.Italiano));
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is SaveMessage)
        {
            StartCoroutine(SavingCoroutine());
        }
        else if (message is LoadMessage)
        {
            StartCoroutine(LoadingCoroutine());
        }

    }

    public void OnDisableSubscribe()
    {
        Publisher.Unsubscribe(this, typeof(SaveMessage));
        Publisher.Unsubscribe(this, typeof(LoadMessage));
    }

    private void OnDestroy()
    {
        OnDisableSubscribe();
    }

    private IEnumerator SavingCoroutine()
    {
        float currentNtimeScale = Time.timeScale;
        Time.timeScale = 0;
        var savableEntities = FindObjectsOfType<SavableEntity>().ToList();
        SavableInfosList savableInfosList = new();
        savableInfosList.SavableInfos = savableEntities.Select(x => x.SaveInfo()).ToList();
        yield return new WaitUntil(() => FileSystem.SaveJson($"SavedScene_{SceneManager.GetActiveScene().name}", "json", savableInfosList));
        Time.timeScale = currentNtimeScale;
    }

    private IEnumerator LoadingCoroutine()
    {
        SavableInfosList savableEntities = null;
        yield return new WaitUntil(() => FileSystem.LoadJson($"SavedScene_{SceneManager.GetActiveScene().name}", "json", out savableEntities));
        foreach (var savableInfo in savableEntities.SavableInfos)
        {
            if (savableInfo.IsPlayer)
            {
                Debug.Log("Spawn player");
                var player = Instantiate(PlayerPrefab, new Vector3(savableInfo.xPos, savableInfo.yPos, savableInfo.zPos), Quaternion.identity);
                List<Hourglass> hourglasses = new List<Hourglass>();
                hourglasses.Add(new Hourglass(savableInfo.currentHourglassLife));
                for (int i = 1; i < savableInfo.hourglassQuantity; i++)
                {
                    hourglasses.Add(new Hourglass(50));
                }
                player.Damageable.SetHourglasses(hourglasses);
                player.Damageable.SetCurrentLife(savableInfo.currentHourglassLife);
            }
            else
            {
                Debug.Log("Spawn enemy");
            }
        }
        //_gameObject.transform.position = new Vector3(savableEntities.SavableInfos[0].xPos, savableEntities.SavableInfos[0].yPos, savableEntities.SavableInfos[0].zPos);
        //_gameObject2.transform.position = new Vector3(savableEntities.SavableInfos[1].xPos, savableEntities.SavableInfos[1].yPos, savableEntities.SavableInfos[1].zPos);
    }

    public void EnablePlayerInputs(bool enable)
    {
        if (enable)
            inputSystem.Player.Enable();
        else
            inputSystem.Player.Disable();
    }
}
