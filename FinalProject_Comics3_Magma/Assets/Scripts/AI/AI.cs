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

        }
       
    }
    private void Update()
    {
        if (!Stupid && Agent.velocity.magnitude > 0.01f)
            CurrentDirection = Agent.velocity.CalculateDirection();
    }
}
