using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
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
    // Corrected Variables
    private InputSystem inputSystem;
    private GhostManager ghostManager;

    // Properties
    public PlayerController Player => playerController;
    public List<PlayerController> PlayerGhostControllerList;
    public GhostManager GhostManager => ghostManager;
    Coroutine ghostEffect;
    private void Awake()
    {
        instance = this;

        ghostManager = GetComponent<GhostManager>();
        ghostManager.Initialize(playerController);
        PlayerGhostControllerList = new();
        //if (playerMovement == null)
        //    throw new Exception("PlayerMovement assente nella scena attuale, importare il prefab del player!!!");

        // INPUT SYSTEM

        inputSystem = new InputSystem();
        inputSystem.Player.Enable();
        inputSystem.Player.Movement.performed += Movement_started;
        inputSystem.Player.Movement.canceled += Movement_canceled;
        inputSystem.Player.MovementWASD.performed += Movement_started;
        inputSystem.Player.MovementWASD.canceled += Movement_canceled;
        inputSystem.Player.Attack.performed += Attack_performed;
        inputSystem.Player.Dash.performed += Dash_performed;
        inputSystem.Player.Rewind.performed += Rewind_performed;
        inputSystem.Player.Pause.performed += Pause_performed;
        inputSystem.Player.ActiveObjectOne.performed += ActiveObjectOnePerformed;
        inputSystem.Player.ActiveObjectTwo.performed += ActiveObjectTwoPerformed;
        inputSystem.Player.ActiveObjectThree.performed += ActiveObjectThreePerformed;
        inputSystem.Player.ActiveObjectFour.performed += ActiveObjectFourPerformed;
        // END INPUT SYSTEM

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
            PlayerGhostControllerList.ForEach(gh => Destroy(gh.gameObject));
            PlayerGhostControllerList.Clear();

            globalVolume.gameObject.SetActive(false);
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
            globalVolume.weight = Mathf.Lerp(globalVolume.weight, 0, weightSpeed);
            yield return null;

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
            inputSystem.Player.Disable();
    }

    public void ChangeScene(string sceneName)
    {
        EnablePlayerInputs(false);
        LevelManager.Instance.LoadScene(sceneName);
        Destroy(gameObject);
    }
}
