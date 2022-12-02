using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [HideInInspector] public BehaviorTree BehaviorTree;
    [HideInInspector] public NavMeshAgent Agent;
    private void Awake()
    {
        BehaviorTree = gameObject.SearchComponent<BehaviorTree>();
        Agent = gameObject.SearchComponent<NavMeshAgent>();
        if(BehaviorTree != null && Agent != null)
        {
            BehaviorTree.SetVariableValue("StoppingDistance", Agent.stoppingDistance);
        }
       
    }
}
