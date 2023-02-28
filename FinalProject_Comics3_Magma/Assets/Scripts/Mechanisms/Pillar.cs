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
    [HideInInspector] public SpriteRenderer sprite;

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
            sprite = GetComponent<SpriteRenderer>();
        }           
    }

    public void PillarDestruction()
    {
        if (pillarHandler != null && pillarState == EPillarState.Active) 
        {
            Debug.Log("Pilastro distrutto");

            sprite.color = Color.red;

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
        sprite.color = Color.blue;
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
