using BehaviorDesigner.Runtime.Tasks;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;

public enum EPillarState { Active, Destroyed, Reactivation, Inactive }

public class Pillar : MonoBehaviour
{
    [SerializeField] PillarHandler pillarHandler;
    /*[HideInInspector]*/ public EPillarState pillarState = EPillarState.Active;

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
            pillarHandler.RemovePillar(this);

            Debug.Log("Pilastro distrutto");

            pillarHandler.CheckPillars();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(pillarHandler != null)
            Gizmos.DrawLine(transform.position, pillarHandler.transform.position);
    }
}
