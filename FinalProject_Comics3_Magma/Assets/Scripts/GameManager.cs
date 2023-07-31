using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Action = System.Action;

public class GameManager : MonoBehaviour
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
    //[SerializeField] PlayerManager PlayerPrefab;
    [SerializeField] PlayerController playerController;

    [SerializeField] Volume globalVolume;
    [SerializeField] float speedGhostEffect = 0.001f;
    [SerializeField] float weightSpeed = 0.001f;
    [SerializeField] GameObject cameraGameOver;
    [SerializeField] SaveSO saveAsset;
    // Corrected Variables
    private InputSystem inputSystem;
    private GhostManager ghostManager;

    // Properties
    public PlayerController Player => playerController;
    public List<PlayerController> PlayerGhostControllerList;
    public GhostManager GhostManager => ghostManager;

    public bool MessageActive { get; set; }
    public bool DialogueMessageActive { get; set; }

    Coroutine ghostEffect;
    bool gamePaused;
    bool gameStart;
    private void Awake()
    {
        instance = this;

        playerController ??= FindObjectOfType<PlayerController>(true);
        globalVolume ??= FindObjectOfType<Volume>(true);

        ghostManager = GetComponent<GhostManager>();
        ghostManager.Initialize(playerController);
        PlayerGhostControllerList = new();
        //if (playerMovement == null)
        //    throw new Exception("PlayerMovement assente nella scena attuale, importare il prefab del player!!!");

        // INPUT SYSTEM
        gamePaused = false;
        inputSystem = new InputSystem();
        inputSystem.Player.Movement.performed += Movement_started;
        inputSystem.Player.Movement.canceled += Movement_canceled;
        inputSystem.Player.MovementWASD.performed += Movement_started;
        inputSystem.Player.MovementWASD.canceled += Movement_canceled;
        inputSystem.Player.Attack.performed += Attack_performed;
        inputSystem.Player.AttackMouse.performed += Attack_performed;
        inputSystem.Player.Dash.performed += Dash_performed;
        inputSystem.Player.Rewind.performed += Rewind_performed;
        inputSystem.Player.Pause.performed += Pause_performed;
        inputSystem.Player.ActiveObjectOne.performed += ActiveObjectOnePerformed;
        inputSystem.Player.ActiveObjectTwo.performed += ActiveObjectTwoPerformed;
        inputSystem.Player.ActiveObjectThree.performed += ActiveObjectThreePerformed;
        inputSystem.Player.ActiveObjectFour.performed += ActiveObjectFourPerformed;
        // END INPUT SYSTEM

        EnableCursor(false);
    }

    private void Start()
    {
        if(saveAsset.SceneName != string.Empty && saveAsset.LastCheckPointPosition != Vector3.zero)
            Player.PlayerManager.Respawn(saveAsset.LastCheckPointPosition);

        gameStart = true;
        StartCoroutine(WaitForEndOfFrameCoroutine(() => EnablePlayerInputs(true)));
    }

    private IEnumerator WaitForEndOfFrameCoroutine(Action action)
    {
        yield return new WaitForEndOfFrame();
        gameStart = false;
        action.Invoke();
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
        gamePaused = !gamePaused;
        UIManager.Instance.Pause();
    }

    private void Rewind_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (gamePaused)
            return;

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

    public void EndGhost()
    {

        GhostManager.RegistraInput(Vector2.zero, InputType.Ghost);
        EnableGhostRendering();
    }

    public void EnableGhostRendering()
    {
        if (playerController.GhostActive)
        {
            globalVolume.gameObject.SetActive(true);
            UIManager.Instance.UIPlayArea.EnableQMessage(true);

            ghostEffect = StartCoroutine(GhostEffect());
        }
        else
        {
            PlayerGhostControllerList.ForEach(gh => Destroy(gh.gameObject));
            PlayerGhostControllerList.Clear();

            globalVolume.gameObject.SetActive(false);
            UIManager.Instance.UIPlayArea.EnableQMessage(false);
            StopCoroutine(ghostEffect);
        }
    }

    private IEnumerator GhostEffect()
    {
        globalVolume.profile.TryGet<ChromaticAberration>(out var type);
        globalVolume.weight = 1;

        var ghostTime = playerController.GetGhostLifeTime();

        float intensity = 0.5f;
        bool goingUp = true;
        bool goingDown = false;
        while (true)
        {
            type.intensity.Override(intensity);
            globalVolume.weight = Mathf.Lerp(globalVolume.weight, 0.15f, weightSpeed - (ghostTime * weightSpeed / 100));
            yield return new WaitForEndOfFrame();

            if(goingDown)
            { 
                intensity -= speedGhostEffect;
                if (intensity <= 0.5f)
                {
                    goingUp = true;
                    goingDown = false;
                }
            }
            else if(goingUp)
            {
                intensity += speedGhostEffect;
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
        if (gamePaused)
            return;

        if (playerController.GhostActive)
        {
            GhostManager.RegistraInput(Vector2.zero, InputType.Dash);

            PlayerGhostControllerList.ForEach(pG =>
            {
                pG.Dash();
            });
        }

        playerController.Dash();
    }

    private void Movement_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (playerController.GhostActive)
        {
            GhostManager.RegistraInput(obj.ReadValue<Vector2>(), InputType.MovementEnd);

            PlayerGhostControllerList.ForEach(pG =>
            {
                pG.Direction = obj.ReadValue<Vector2>();
            });
        }

        playerController.Direction = obj.ReadValue<Vector2>();
    }

    private void Movement_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (gamePaused)
            return;

        Vector2 readedDirection = obj.ReadValue<Vector2>();

        playerController.Direction = readedDirection.normalized;
        playerController.lastDirection = readedDirection.normalized;

        if (playerController.GhostActive)
        {
            GhostManager.RegistraInput(obj.ReadValue<Vector2>(), InputType.MovementStart);

            PlayerGhostControllerList.ForEach(pG =>
            {
                pG.Direction = readedDirection.normalized;
                pG.lastDirection = readedDirection.normalized;
                pG.StateMachine.SetState(EPlayerState.Walking);
            });
        }

        if (playerController.CanMove)
        {
            playerController.StateMachine.SetState(EPlayerState.Walking);           
        }      
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (MessageActive)
        {
            if(UIManager.Instance.CloseWrittenPanelByContinueButton())
                return;
        }

        if (DialogueMessageActive)
        {
            UIManager.Instance.NextDialogue();
            return;
        }

        if (gamePaused)
            return;

        if (playerController.GhostActive)
        {
            GhostManager.RegistraInput(Vector2.zero, InputType.Attack);

            PlayerGhostControllerList.ForEach(pG =>
            {
                if (!pG.IsAttacking)
                {
                    pG.Attack();
                }
            });
        }

        if(!playerController.IsAttacking) 
            playerController.Attack();
    }


    public void EnablePlayerInputs(bool enable)
    {
        if (enable)
            inputSystem.Player.Enable();
        else
        {
            inputSystem.Player.Movement.Disable();
            inputSystem.Player.MovementWASD.Disable();

            inputSystem.Player.Dash.Disable();
            inputSystem.Player.Rewind.Disable();
            inputSystem.Player.Pause.Disable();
            inputSystem.Player.ActiveObjectOne.Disable();
            inputSystem.Player.ActiveObjectTwo.Disable();
            inputSystem.Player.ActiveObjectThree.Disable();
            inputSystem.Player.ActiveObjectFour.Disable();
        }
    }

    public void EnablePlayerInputs(bool enable, bool overrideAttack = false)
    {
        if (enable)
            inputSystem.Player.Enable();
        else
        {
            inputSystem.Player.Movement.Disable();
            inputSystem.Player.MovementWASD.Disable();

            inputSystem.Player.Dash.Disable();
            inputSystem.Player.Rewind.Disable();
            inputSystem.Player.Pause.Disable();
            inputSystem.Player.ActiveObjectOne.Disable();
            inputSystem.Player.ActiveObjectTwo.Disable();
            inputSystem.Player.ActiveObjectThree.Disable();
            inputSystem.Player.ActiveObjectFour.Disable();

            if (overrideAttack)
            {
                inputSystem.Player.Attack.Disable();
                inputSystem.Player.AttackMouse.Disable();
            }
        }
    }

    public void EnablePauseMenuInput(bool enabled)
    {
        if (enabled)
            inputSystem.Player.Pause.Enable();
        else
            inputSystem.Player.Pause.Disable();
    }

    public void ChangeScene(string sceneName)
    {
        EnablePlayerInputs(false);
        inputSystem.Player.Attack.Disable();
        inputSystem.Player.AttackMouse.Disable();
        StartCoroutine(WaitForEndOfFrameCoroutine(() => Destroy(gameObject)));
        LevelManager.Instance.LoadScene(sceneName);
    }

    public void EnableCursor(bool enabled)
    {
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enabled;
    }

    public void GameOver()
    {
        EnableCursor(true);
        UIManager.Instance.OpenGameOverPanel();
    }

    public void EnableCameraGameOver(bool enabled)
    {
        cameraGameOver.SetActive(enabled);
    }

    public void Save()
    {
        saveAsset.SceneName = SceneManager.GetActiveScene().name;
    }

    public void SetCheckPoint(CheckPoint checkPoint)
    {
        saveAsset.LastCheckPointPosition = checkPoint.transform.position;
        if (gameStart)
            checkPoint.LoadRooms();
    }

    public void Load()
    {
        ChangeScene(saveAsset.SceneName);
    }
}
