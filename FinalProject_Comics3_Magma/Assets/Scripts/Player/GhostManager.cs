using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType { Dash, Ghost, MovementStart, MovementEnd, Attack, Interaction }

public class GhostManager : MonoBehaviour
{
    //Dictionary<InputType, UnityEngine.InputSystem.InputAction.CallbackContext> ghostDictionary;
    Queue<InputType> inputTypes = new Queue<InputType>();
    Queue<UnityEngine.InputSystem.InputAction.CallbackContext> callbackContexts = new Queue<UnityEngine.InputSystem.InputAction.CallbackContext>();
    List<float> timeDistance = new List<float>();

    bool readTime = false;
    bool readInput = false;

    float timeElapsed = 0;
    float currentTimeElapsed = 0;

    [SerializeField] PlayerController ghost;

    // Start is called before the first frame update
    void Start()
    {
        //ghostDictionary = new Dictionary<InputType, UnityEngine.InputSystem.InputAction.CallbackContext>();
    }

    // Update is called once per frame
    void Update()
    {
        if(readInput)
        {
            currentTimeElapsed += Time.deltaTime;

            if(currentTimeElapsed >= timeDistance[0] ) 
            { 
                timeDistance.RemoveAt(0);
                EseguiInputGhost();
            }
        }
        else if (readTime)
        {
            timeElapsed += Time.deltaTime;
        }
    }

    private void EseguiInputGhost()
    {
        var input = inputTypes.Dequeue();
        var callback = callbackContexts.Dequeue();

        switch (input)
        {
            case InputType.Dash:
                ghost.Dash();
                break;
            case InputType.Ghost:
                ResetGhost();

                break;
            case InputType.MovementStart:
                if (!ghost.IsDashing)
                {
                    ghost.StateMachine.SetState(EPlayerState.Walking);

                    Vector2 readedDirection = callback.ReadValue<Vector2>();

                    ghost.Direction = readedDirection.normalized;
                    ghost.lastDirection = readedDirection.normalized;
                }
                break;
            case InputType.MovementEnd:
                ghost.Direction = callback.ReadValue<Vector2>();
                break;
            case InputType.Attack:
                ghost.Attack();
                break;
            case InputType.Interaction:
                break;
        }

        
    }

    public void ResetGhost()
    {
        readInput = false;
        Destroy(ghost.gameObject);

        inputTypes.Clear();
        callbackContexts.Clear();
        timeDistance.Clear();

        timeElapsed = 0;
        currentTimeElapsed = 0;
    }

    public void RegistraInput(UnityEngine.InputSystem.InputAction.CallbackContext obj, InputType inputType)
    {
        inputTypes.Enqueue(inputType);
        callbackContexts.Enqueue(obj);
        timeDistance.Add(timeElapsed);
    }

    public void StartExecuteInputs()
    {
        readInput = true;
        readTime = false;
    }

    public void StartReadingDistance(PlayerController newGhost)
    {
        ghost = newGhost;

        timeDistance.Clear();
        timeElapsed = 0;

        readTime = true;       
    }

}
