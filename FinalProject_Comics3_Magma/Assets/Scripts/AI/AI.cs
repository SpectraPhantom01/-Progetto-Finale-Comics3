using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [HideInInspector] public BehaviorTree BehaviorTree;
    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public EDirection CurrentDirection = EDirection.Down;
    public bool Stupid;
    public Damageable Damageable { get; private set; }

    [Header("SFX")]
    public GameObject SoundToSpawnOnKillPrefab;
    private void Awake()
    {
        if(!Stupid)
        {
            BehaviorTree = gameObject.SearchComponent<BehaviorTree>();
            Agent = gameObject.SearchComponent<NavMeshAgent>();
            if (BehaviorTree != null && Agent != null)
            {
                BehaviorTree.SetVariableValue("StoppingDistance", Agent.stoppingDistance);
            }

            Damageable = gameObject.SearchComponent<Damageable>();
        }
    }
    private void Update()
    {
        if (!Stupid && Agent.velocity.magnitude > 0.01f)
            CurrentDirection = Agent.velocity.CalculateDirection(CurrentDirection);
    }
    public void Stop()
    {
        BehaviorTree.enabled = false;
        Agent.enabled = false;
    }
    public void Play()
    {
        BehaviorTree.enabled = true;
        Agent.enabled = true;
    }
}
