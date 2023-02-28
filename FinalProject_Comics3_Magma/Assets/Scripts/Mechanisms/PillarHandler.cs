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

    List<Pillar> pillarsList = new List<Pillar>();

    private void Start()
    {
        if (pillarsList.Count == 0)
            Destroy(gameObject);
    }

    //private void Update()
    //{
    //    
    //}

    public void CheckPillars()
    {
        if (pillarsList.All(p => p.GetPillarState() == EPillarState.Destroyed))
        {
            Event?.Invoke();
            Debug.Log("=== Attivazione evento ===");

            Deactivation();
        }
    }

    private void Deactivation()
    {
        foreach (Pillar pillar in pillarsList)
        {
            pillar.SetPillarState(EPillarState.Inactive);
            pillar.StopCoroutine(pillar.pillarRoutine);

            pillar.sprite.color = Color.black;
        }
    }

    public void AddPillar(Pillar pillar)
    {
        pillarsList.Add(pillar);
    }

    public void RemovePillar(Pillar pillar)
    {
        pillarsList.Remove(pillar);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }


}
