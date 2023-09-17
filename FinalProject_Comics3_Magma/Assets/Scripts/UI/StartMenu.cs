using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour, ISubscriber
{
    [SerializeField] SaveSO saveAsset;
    [SerializeField] Button continueButton;

    InputSystem inputAction;
    private GamePadMouseHandler gamePadMouseHandler;
    private void Awake()
    {
        Publisher.Subscribe(this, typeof(InputDeviceChangedMessage));

        gamePadMouseHandler = GetComponent<GamePadMouseHandler>();

        if(!File.Exists(Application.persistentDataPath + "\\save.txt"))
        {
            File.Create(Application.persistentDataPath + "\\save.txt");
        }
    }

    private void Start()
    {
        var saveAssetJson = File.ReadAllText(Application.persistentDataPath + "\\save.txt");
        if(saveAssetJson != null)
        {
            var so = JsonUtility.FromJson<SaveJSON>(saveAssetJson);

            if (so != null)
            {
                saveAsset.SceneName = so.SceneName;
                saveAsset.LastCheckPointPosition = so.LastCheckPointPosition;
                continueButton.interactable = true;
            }

        }

        SetInputToPads();
    }

    private void SetInputToPads()
    {
        inputAction = new InputSystem();

        if (LevelManager.Instance.JoyStickInputAvailable)
        {
            inputAction.PlayerUSBJoyStick.Enable();
            inputAction.PlayerUSBJoyStick.UIMouseMovement.performed += UIMouseMovement_performed;
            inputAction.PlayerUSBJoyStick.UIMouseMovement.canceled += UIMouseMovement_performed;
            inputAction.PlayerUSBJoyStick.UIMouseClick.performed += UIMouseClick_performed;
        }

        if (LevelManager.Instance.GamePadInputAvailable)
        {
            inputAction.PlayerGamePad.Enable();
            inputAction.PlayerGamePad.UIMouseMovement.performed += UIMouseMovement_performed;
            inputAction.PlayerGamePad.UIMouseMovement.canceled += UIMouseMovement_performed;
            inputAction.PlayerGamePad.UIMouseClick.performed += UIMouseClick_performed;
        }

        gamePadMouseHandler.Active = LevelManager.Instance.GamePadInputAvailable || LevelManager.Instance.JoyStickInputAvailable;
    }

    private void UIMouseClick_performed(InputAction.CallbackContext obj)
    {
        if (LevelManager.Instance.JoyStickInputAvailable || LevelManager.Instance.GamePadInputAvailable)
            gamePadMouseHandler.Click();
    }

    private void UIMouseMovement_performed(InputAction.CallbackContext obj)
    {
        if (LevelManager.Instance.JoyStickInputAvailable || LevelManager.Instance.GamePadInputAvailable)
            gamePadMouseHandler.SetDirection(obj.ReadValue<Vector2>());
    }

    private void OnDestroy()
    {
        inputAction?.Disable();
    }

    private void OnDisable()
    {
        OnDisableSubscribe();
        inputAction?.Disable();
    }

    public void ContinueGame()
    {
        LevelManager.Instance.LoadScene(saveAsset.SceneName);
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is InputDeviceChangedMessage)
        {
            SetInputToPads();
        }
    }

    public void OnDisableSubscribe()
    {
        Publisher.Unsubscribe(this, typeof(InputDeviceChangedMessage));
    }
}
