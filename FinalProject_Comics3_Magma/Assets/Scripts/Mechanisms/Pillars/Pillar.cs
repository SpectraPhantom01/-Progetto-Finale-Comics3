using BehaviorDesigner.Runtime.Tasks;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EPillarState { Active, Destroyed, Reactivation, Inactive } //Enum necessario o gestione con bool?

public class Pillar : MonoBehaviour
{
    [SerializeField] PillarHandler pillarHandler;
    [SerializeField] float pillarDestroyedTime;
    [SerializeField] bool endless;
    [SerializeField] Color activeColor;
    [SerializeField] Color inactiveColor;
    [SerializeField] UnityEvent onHitPillar;
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
            sprite = GetComponentInChildren<SpriteRenderer>();
            if (endless)
            {
                sprite.color = Color.cyan;
                inactiveColor = Color.cyan;
            }
        }           
    }

    public void PillarDestruction()
    {
        if (pillarHandler != null && pillarState == EPillarState.Active) 
        {
            sprite.color = activeColor;

            pillarState = EPillarState.Destroyed;

            onHitPillar?.Invoke();

            if (!endless)
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
        pillarState = EPillarState.Active;
        sprite.color = inactiveColor;
    }

    public EPillarState GetPillarState()
    {
        return pillarState;
    }

    public void SetPillarState(EPillarState state)
    {
        pillarState = state;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if(pillarHandler != null)
            Gizmos.DrawLine(transform.position, pillarHandler.transform.position);
    }
#endif
}
