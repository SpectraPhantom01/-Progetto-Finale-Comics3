using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneManager : MonoBehaviour
{
    InputSystem inputActions;
    [SerializeField] UIDialoguePanel UIDialoguePanel;
    private void Awake()
    {
        inputActions = new InputSystem();
        
        inputActions.Player.Attack.Enable();
        inputActions.Player.Attack.performed += Attack_performed;

        if (LevelManager.FindInputDevice("gamepoad"))
        {
            inputActions.PlayerGamePad.Attack.Enable();
            inputActions.PlayerGamePad.Attack.performed += Attack_performed;
        }

        if (LevelManager.FindInputDevice("joystick"))
        {
            inputActions.PlayerUSBJoyStick.Attack.Enable();
            inputActions.PlayerUSBJoyStick.Attack.performed += Attack_performed;
        }
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UIDialoguePanel.Next();
    }

    public void Disable()
    {
        inputActions.Disable();
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }

    public void LoadScene(string sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
    }
}
