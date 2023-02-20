using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public bool CheckPillars()
    {
        if (pillarsList.All(p => p.pillarState == EPillarState.Destroyed))
        {
            //Event.Invoke();
            Debug.Log("=== Attivazione evento ===");

            foreach (Pillar pillar in pillarsList)
            {
                pillar.pillarState = EPillarState.Inactive;
                pillar.StopCoroutine(pillar.pillarRoutine);
            }
            return false;
        }
        return true;
    }

    //public void RemovePillar(Pillar pillar)
    //{
    //    //pillarsList.Remove(pillar);
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(gameObject.transform.position, 0.1f);
    }


}
