using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Action = System.Action;

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
    private GamePadMouseHandler gamePadMouseHandler;
    // Properties
    public PlayerController Player => playerController;
    public List<PlayerController> PlayerGhostControllerList;
    public GhostManager GhostManager => ghostManager;

    public bool MessageActive { get; set; }
    public bool DialogueMessageActive { get; set; }

    Coroutine ghostEffect;
    bool gamePaused;
    bool gameStart;
    bool gamePadInput = false;
    bool joyStickInput = false;
    bool mouseAndKeyboardInput = true;
    private void Awake()
    {
        instance = this;

        Publisher.Subscribe(this, typeof(InputDeviceChangedMessage));

        gamePadMouseHandler = GetComponent<GamePadMouseHandler>();

        playerController ??= FindObjectOfType<PlayerController>(true);
        globalVolume ??= FindObjectOfType<Volume>(true);

        ghostManager = GetComponent<GhostManager>();
        ghostManager.Initialize(playerController);
        PlayerGhostControllerList = new();

        EnableDevices();

        gamePaused = false;
        BuildInputs();

        EnableCursor(false);
    }


    private void EnableDevices()
    {
        joyStickInput = LevelManager.Instance.JoyStickInputAvailable;
        mouseAndKeyboardInput = LevelManager.Instance.MouseAndKeyboardInputAvailable;
        gamePadInput = LevelManager.Instance.GamePadInputAvailable;
    }

    private void BuildInputs()
    {
        inputSystem = new InputSystem();

        inputSystem.PlayerGamePad.Movement.performed += Movement_started;
        inputSystem.PlayerGamePad.Movement.canceled += Movement_canceled;
        inputSystem.PlayerGamePad.Attack.performed += Attack_performed;
        inputSystem.PlayerGamePad.Dash.performed += Dash_performed;
        inputSystem.PlayerGamePad.Rewind.performed += Rewind_performed;
        inputSystem.PlayerGamePad.Pause.performed += Pause_performed;
        inputSystem.PlayerGamePad.ActiveObjectOne.performed += ActiveObjectOnePerformed;
        inputSystem.PlayerGamePad.ActiveObjectTwo.performed += ActiveObjectTwoPerformed;
        inputSystem.PlayerGamePad.ActiveObjectThree.performed += ActiveObjectThreePerformed;
        inputSystem.PlayerGamePad.ActiveObjectFour.performed += ActiveObjectFourPerformed;

        inputSystem.PlayerUSBJoyStick.Movement.performed += Movement_started;
        inputSystem.PlayerUSBJoyStick.Movement.canceled += Movement_canceled;
        inputSystem.PlayerUSBJoyStick.Attack.performed += Attack_performed;
        inputSystem.PlayerUSBJoyStick.Dash.performed += Dash_performed;
        inputSystem.PlayerUSBJoyStick.Rewind.performed += Rewind_performed;
        inputSystem.PlayerUSBJoyStick.Pause.performed += Pause_performed;
        inputSystem.PlayerUSBJoyStick.ActiveObjectOne.performed += ActiveObjectOnePerformed;
        inputSystem.PlayerUSBJoyStick.ActiveObjectTwo.performed += ActiveObjectThreePerformed;
        inputSystem.PlayerUSBJoyStick.ActiveObjectThree.performed += ActiveObjectTwoPerformed;
        inputSystem.PlayerUSBJoyStick.ActiveObjectFour.performed += ActiveObjectFourPerformed;

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
        inputSystem.Player.ActiveObjectTwo.performed += ActiveObjectThreePerformed;
        inputSystem.Player.ActiveObjectThree.performed += ActiveObjectTwoPerformed;
        inputSystem.Player.ActiveObjectFour.performed += ActiveObjectFourPerformed;
    }

    private void UIMouseMovement_performed(InputAction.CallbackContext obj)
    {
        
       gamePadMouseHandler.SetDirection(obj.ReadValue<Vector2>());

    }

    private void Start()
    {
        if(saveAsset.SceneName != string.Empty 
            && SceneManager.GetActiveScene().name == saveAsset.SceneName 
            && saveAsset.LastCheckPointPosition != Vector3.zero)
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

        if(joyStickInput)
        {
            gamePadMouseHandler.Active = gamePaused;
            if (gamePaused)
            {
                inputSystem.PlayerUSBJoyStick.UIMouseMovement.performed += UIMouseMovement_performed;
                inputSystem.PlayerUSBJoyStick.UIMouseMovement.canceled += UIMouseMovement_performed;
                inputSystem.PlayerUSBJoyStick.UIMouseClick.performed += UIMouseClick_performed;
            }
            else
            {
                inputSystem.PlayerUSBJoyStick.UIMouseMovement.performed -= UIMouseMovement_performed;
                inputSystem.PlayerUSBJoyStick.UIMouseMovement.canceled -= UIMouseMovement_performed;
                inputSystem.PlayerUSBJoyStick.UIMouseClick.performed -= UIMouseClick_performed;
            }
        }

        if(gamePadInput)
        {
            gamePadMouseHandler.Active = gamePaused;
            if (gamePaused)
            {
                inputSystem.PlayerGamePad.UIMouseMovement.performed += UIMouseMovement_performed;
                inputSystem.PlayerGamePad.UIMouseMovement.canceled += UIMouseMovement_performed;
                inputSystem.PlayerUSBJoyStick.UIMouseClick.performed += UIMouseClick_performed;
            }
            else
            {
                inputSystem.PlayerGamePad.UIMouseMovement.performed -= UIMouseMovement_performed;
                inputSystem.PlayerGamePad.UIMouseMovement.canceled -= UIMouseMovement_performed;
                inputSystem.PlayerUSBJoyStick.UIMouseClick.performed -= UIMouseClick_performed;
            }
        }
    }

    private void UIMouseClick_performed(InputAction.CallbackContext obj)
    {
        gamePadMouseHandler.Click();
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
        {
            if(mouseAndKeyboardInput)
                inputSystem.Player.Enable();
            
            if(gamePadInput)
                inputSystem.PlayerGamePad.Enable();
            
            if(joyStickInput)
                inputSystem.PlayerUSBJoyStick.Enable();
        }
        else
        {
            if (mouseAndKeyboardInput)
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

            if (gamePadInput)
            {
                inputSystem.PlayerGamePad.Movement.Disable();
                inputSystem.PlayerGamePad.Dash.Disable();
                inputSystem.PlayerGamePad.Rewind.Disable();
                inputSystem.PlayerGamePad.Pause.Disable();
                inputSystem.PlayerGamePad.ActiveObjectOne.Disable();
                inputSystem.PlayerGamePad.ActiveObjectTwo.Disable();
                inputSystem.PlayerGamePad.ActiveObjectThree.Disable();
                inputSystem.PlayerGamePad.ActiveObjectFour.Disable();
            }

            if (joyStickInput)
            {
                inputSystem.PlayerUSBJoyStick.Movement.Disable();
                inputSystem.PlayerUSBJoyStick.Dash.Disable();
                inputSystem.PlayerUSBJoyStick.Rewind.Disable();
                inputSystem.PlayerUSBJoyStick.Pause.Disable();
                inputSystem.PlayerUSBJoyStick.ActiveObjectOne.Disable();
                inputSystem.PlayerUSBJoyStick.ActiveObjectTwo.Disable();
                inputSystem.PlayerUSBJoyStick.ActiveObjectThree.Disable();
                inputSystem.PlayerUSBJoyStick.ActiveObjectFour.Disable();
            }
        }
    }

    public void EnablePlayerInputs(bool enable, bool overrideAttack = false)
    {
        EnablePlayerInputs(enable);

        if (!enable && overrideAttack)
        {
            inputSystem.Player.Attack.Disable();
            inputSystem.Player.AttackMouse.Disable();
            inputSystem.PlayerGamePad.Attack.Disable();
            inputSystem.PlayerUSBJoyStick.Attack.Disable();
        }
    }

    public void EnablePauseMenuInput(bool enabled)
    {
        if (enabled)
        {
            if (mouseAndKeyboardInput)
                inputSystem.Player.Enable();

            if (gamePadInput)
                inputSystem.PlayerGamePad.Enable();

            if (joyStickInput)
                inputSystem.PlayerUSBJoyStick.Enable();
        }
        else
        {
            if (mouseAndKeyboardInput)
                inputSystem.Player.Disable();

            if (gamePadInput)
                inputSystem.PlayerGamePad.Disable();

            if (joyStickInput)
                inputSystem.PlayerUSBJoyStick.Disable();
        }
    }

    public void ChangeScene(string sceneName)
    {
        EnablePlayerInputs(false);

        if (mouseAndKeyboardInput)
        {
            inputSystem.Player.Attack.Disable();
            inputSystem.Player.AttackMouse.Disable();
        }
        if (gamePadInput)
            inputSystem.PlayerGamePad.Attack.Disable();
        if (joyStickInput)
            inputSystem.PlayerUSBJoyStick.Attack.Disable();

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

    public void OnPublish(IPublisherMessage message)
    {
        if(message is InputDeviceChangedMessage)
        {
            EnableDevices();
        }
    }

    public void OnDisableSubscribe()
    {
        Publisher.Unsubscribe(this, typeof(InputDeviceChangedMessage));
    }
    private void OnDestroy()
    {
        OnDisableSubscribe();
    }
}
