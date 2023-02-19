using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PillarHandler : MonoBehaviour
{
    [Header("Event List")]
    [Space(5)]
    public UnityEvent Event;

    [HideInInspector] public List<Pillar> pillarsList;

    //private void Update()
    //{
    //    
    //}

    public void CheckPillars()
    {
        if (pillarsList.Count == 0)
        {
            //Event.Invoke();
            Debug.Log("=== Attivazione evento ===");

            //foreach (Pillar pillar in pillarsList)
            //{
            //    pillar.pillarState = EPillarState.Inactive;
            //}
        }
            
    }

    public void RemovePillar(Pillar pillar)
    {
        //pillarsList.Remove(pillar);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(gameObject.transform.position, 0.1f);
    }


}
