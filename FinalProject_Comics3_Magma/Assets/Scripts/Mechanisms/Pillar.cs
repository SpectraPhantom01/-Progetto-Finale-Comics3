using BehaviorDesigner.Runtime.Tasks;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public enum EPillarState { Active, Destroyed, Reactivation, Inactive } //Enum necessario o gestione con bool?

public class Pillar : MonoBehaviour
{
    [SerializeField] PillarHandler pillarHandler;
    [SerializeField] float pillarDestroyedTime;
    EPillarState pillarState = EPillarState.Active;
    //bool destroyed = false;

    public IEnumerator pillarRoutine;

    private void Awake()
    {
        if (pillarHandler == null)
        {
            /*Debug.LogError("Attenzione! Pilastro non associato ad un Pillar Handler!");*/ // Distruggere pilastro se non associato a nessuno handler?
            Destroy(gameObject); 
        }
        else
        {
            pillarHandler.AddPillar(this);
        }           
    }

    public void PillarDestruction()
    {
        if (pillarHandler != null && pillarState == EPillarState.Active) // Da sistemare if
        {
            Debug.Log("Pilastro distrutto");

            pillarState = EPillarState.Destroyed;
            ActiveRoutine();
            pillarHandler.CheckPillars();
            
        }
    }

    private void ActiveRoutine()
    {
        pillarRoutine = PillarRoutine();
        StartCoroutine(pillarRoutine);
    }

    private IEnumerator PillarRoutine()
    {
        yield return new WaitForSeconds(pillarDestroyedTime);
        Debug.Log("Pilastro riattivato");
        pillarState = EPillarState.Active;
    }

    public EPillarState GetPillarState()
    {
        return pillarState;
    }

    public void SetPillarState(EPillarState state)
    {
        pillarState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if(pillarHandler != null)
            Gizmos.DrawLine(transform.position, pillarHandler.transform.position);
    }
}
