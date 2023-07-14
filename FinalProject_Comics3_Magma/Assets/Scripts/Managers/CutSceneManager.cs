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
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        UIDialoguePanel.Next();
    }

    private void OnDisable()
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
