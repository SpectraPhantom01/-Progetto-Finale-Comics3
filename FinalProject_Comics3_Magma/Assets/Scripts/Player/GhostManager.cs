using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType { Dash, Ghost, MovementStart, MovementEnd, Attack, Interaction }

public class GhostManager : MonoBehaviour
{
    //Dictionary<InputType, UnityEngine.InputSystem.InputAction.CallbackContext> ghostDictionary;
    Queue<InputType> inputTypes = new Queue<InputType>();
    Queue<Vector2> callbackContexts = new Queue<Vector2>();
    List<float> timeDistance = new List<float>();

    bool readTime = false;
    bool readInput = false;

    float timeElapsed = 0;
    float currentTimeElapsed = 0;

    PlayerController ghost;
    PlayerController player;
    void Update()
    {
        if(readInput)
        {
            currentTimeElapsed += Time.deltaTime;

            if(currentTimeElapsed >= timeDistance[0]) 
            { 
                if(ghost == null) // nel caso in cui il Ghost venga ucciso mentre si sta muovendo... Non si sa mai
                {
                    ResetValues();
                    return;
                }

                timeDistance.RemoveAt(0);
                EseguiInputGhost();
            }
        }
        else if (readTime)
        {
            timeElapsed += Time.deltaTime;
            
            if (ghost == null) // nel caso in cui il Ghost venga ucciso mentre si sta muovendo... Non si sa mai
            {
                ResetValues();
                return;
            }
        }
    }

    public void Initialize(PlayerController player)
    {
        this.player = player;
    }

    private void EseguiInputGhost()
    {
        var input = inputTypes.Dequeue();
        var callbackValue = callbackContexts.Dequeue();

        switch (input)
        {
            case InputType.Dash:
                ghost.Dash();
                break;
            case InputType.Ghost:
                player.DestroyGhost();
                break;
            case InputType.MovementStart:
                if (!ghost.IsDashing)
                {
                    ghost.StateMachine.SetState(EPlayerState.Walking);

                    Vector2 readedDirection = callbackValue;

                    ghost.Direction = readedDirection.normalized;
                    ghost.lastDirection = readedDirection.normalized;
                }
                break;
            case InputType.MovementEnd:
                ghost.Direction = callbackValue;
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
        Destroy(ghost.gameObject);
        ResetValues();
    }

    public void ResetValues()
    {
        readInput = false;

        inputTypes.Clear();
        callbackContexts.Clear();
        timeDistance.Clear();

        timeElapsed = 0;
        currentTimeElapsed = 0;
    }

    public void RegistraInput(Vector2 callBackValue, InputType inputType)
    {
        inputTypes.Enqueue(inputType);
        callbackContexts.Enqueue(callBackValue);
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
