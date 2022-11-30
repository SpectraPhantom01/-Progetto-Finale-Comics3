using System;
using System.Collections;
using System.Linq;
using UnityEngine;
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


    GameObject _gameObject; // debug
    GameObject _gameObject2; // debug

    // Corrected Variables
    private InputSystem inputSystem;
    private PlayerController playerController;
    private LanguageManager languageManager;

    // Properties
    public PlayerController Player => playerController;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        
        playerController = FindObjectOfType<PlayerController>();

        if (playerController == null)
            throw new Exception("PlayerMovement assente nella scena attuale, importare il prefab del player!!!");

        // INPUT SYSTEM

        inputSystem = new InputSystem();
        inputSystem.Player.Enable();
        inputSystem.Player.Movement.performed += Movement_started;
        inputSystem.Player.Movement.canceled += Movement_canceled;
        inputSystem.Player.Attack.performed += Attack_performed;

        // END INPUT SYSTEM



        if (!FileSystem.Load("LanguageManager", "csv", out string[] fileLoaded))
            throw new Exception("File LanguageManager non caricato correttamente, controllare eventuali posizioni del file o il nome del file o l'estensione del file");

        CSVFileReader.Parse(fileLoaded, ';', "", true);

        languageManager = new LanguageManager(CSVFileReader.FileMatrix);

        Publisher.Subscribe(this, typeof(SaveMessage));
        Publisher.Subscribe(this, typeof(LoadMessage));
        _gameObject = new GameObject();
        _gameObject2 = new GameObject();
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.Attack();
    }

    private void Movement_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.Direction = obj.ReadValue<Vector2>();
    }

    private void Movement_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        playerController.Direction = obj.ReadValue<Vector2>();
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
        if (message is SaveMessage saveMessage)
        {
            StartCoroutine(SavingCoroutine());
        }
        else if (message is LoadMessage loadMessage)
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
        SavableInfosList savableInfosList = new SavableInfosList();
        savableInfosList.SavableInfos = savableEntities.Select(x => x.SaveInfo()).ToList();
        yield return new WaitUntil(() => FileSystem.SaveJson($"SavedScene_{SceneManager.GetActiveScene().name}", "json", savableInfosList));
        Time.timeScale = currentNtimeScale;
    }

    private IEnumerator LoadingCoroutine()
    {
        SavableInfosList savableEntities = null;
        yield return new WaitUntil(() => FileSystem.LoadJson($"SavedScene_{SceneManager.GetActiveScene().name}", "json", out savableEntities));
        _gameObject.transform.position = new Vector3(savableEntities.SavableInfos[0].xPos, savableEntities.SavableInfos[0].yPos, savableEntities.SavableInfos[0].zPos);
        _gameObject2.transform.position = new Vector3(savableEntities.SavableInfos[1].xPos, savableEntities.SavableInfos[1].yPos, savableEntities.SavableInfos[1].zPos);
    }
}
