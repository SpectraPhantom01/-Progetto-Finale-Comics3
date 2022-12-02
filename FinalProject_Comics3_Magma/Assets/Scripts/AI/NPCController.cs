using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : AI
{
    [Header("Path")]
    [SerializeField] PatrolPath patrolPath;


    private void Start()
    {
        BehaviorTree.SetVariableValue("PatrolPathPoints", patrolPath.Path);
    }


}
