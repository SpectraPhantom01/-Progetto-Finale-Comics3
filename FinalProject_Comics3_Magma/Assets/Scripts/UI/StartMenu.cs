using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] SaveSO saveAsset;
    [SerializeField] Button continueButton;

    InputSystem inputAction;
    private GamePadMouseHandler gamePadMouseHandler;
    private void Awake()
    {
        if (saveAsset.SceneName != string.Empty)
            continueButton.interactable = true;
        
    }

    private void Start()
    {
        if(LevelManager.Instance.JoyStickInputAvailable)
        {
            gamePadMouseHandler = gameObject.AddComponent<GamePadMouseHandler>();
            inputAction = new InputSystem();
            inputAction.PlayerUSBJoyStick.Enable();
            inputAction.PlayerUSBJoyStick.UIMouseMovement.performed += UIMouseMovement_performed;
            inputAction.PlayerUSBJoyStick.UIMouseMovement.canceled += UIMouseMovement_performed;
            inputAction.PlayerUSBJoyStick.UIMouseClick.performed += UIMouseClick_performed;
            gamePadMouseHandler.Active = true;
        }
        
        if(LevelManager.Instance.GamePadInputAvailable)
        {
            gamePadMouseHandler = gameObject.AddComponent<GamePadMouseHandler>();
            inputAction = new InputSystem();
            inputAction.PlayerGamePad.Enable();
            inputAction.PlayerGamePad.UIMouseMovement.performed += UIMouseMovement_performed;
            inputAction.PlayerGamePad.UIMouseMovement.canceled += UIMouseMovement_performed;
            inputAction.PlayerGamePad.UIMouseClick.performed += UIMouseClick_performed;
            gamePadMouseHandler.Active = true;
        }
    }

    private void UIMouseClick_performed(InputAction.CallbackContext obj)
    {
        gamePadMouseHandler.Click();
    }

    private void UIMouseMovement_performed(InputAction.CallbackContext obj)
    {
        gamePadMouseHandler.SetDirection(obj.ReadValue<Vector2>());
    }

    private void OnDestroy()
    {
        inputAction.Disable();
    }

    private void OnDisable()
    {
        inputAction.Disable();
    }

    public void ContinueGame()
    {
        LevelManager.Instance.LoadScene(saveAsset.SceneName);
    }
}
