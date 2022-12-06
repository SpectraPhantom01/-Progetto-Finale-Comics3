using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [HideInInspector] public BehaviorTree BehaviorTree;
    [HideInInspector] public NavMeshAgent Agent;
    [Space(20)]

    public EDirection CurrentDirection = EDirection.Down;

    private void Awake()
    {
        BehaviorTree = gameObject.SearchComponent<BehaviorTree>();
        Agent = gameObject.SearchComponent<NavMeshAgent>();
        if(BehaviorTree != null && Agent != null)
        {
            BehaviorTree.SetVariableValue("StoppingDistance", Agent.stoppingDistance);
        }
       
    }
    private void Update()
    {
        if (Agent.velocity.magnitude > 0.01f)
            CurrentDirection = Agent.velocity.CalculateDirection();
    }
}
