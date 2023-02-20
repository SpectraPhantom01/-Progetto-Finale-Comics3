using BehaviorDesigner.Runtime.Tasks;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public enum EPillarState { Active, Destroyed, Reactivation, Inactive } //Enum necessario?

public class Pillar : MonoBehaviour
{
    [SerializeField] PillarHandler pillarHandler;
    [SerializeField] float pillarDestroyedTime;
    /*[HideInInspector]*/ public EPillarState pillarState = EPillarState.Active;

    public IEnumerator pillarRoutine;

    private void Awake()
    {
        if (pillarHandler == null)
        {
            Debug.LogError("Attenzione! Pilastro non associato ad un Pillar Handler!"); // Distruggere pilastro se non associato a nessuno handler?
        }
        else
        {
            pillarHandler.pillarsList.Add(this);
        }           
    }

    private void Update()
    {
        
    }

    public void PillarDestruction()
    {
        if (pillarHandler != null && pillarState == EPillarState.Active)
        {
            //Debug.Log("Pilastro distrutto");

            pillarState = EPillarState.Destroyed;

            if (pillarHandler.CheckPillars())
                ActiveRoutine();
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
        pillarState = EPillarState.Active;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(pillarHandler != null)
            Gizmos.DrawLine(transform.position, pillarHandler.transform.position);
    }
}
