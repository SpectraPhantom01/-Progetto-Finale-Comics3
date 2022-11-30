using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] PatrolPath patrolPath;

    BehaviorTree _behaviorTree;

    private void Awake()
    {
        _behaviorTree = gameObject.SearchComponent<BehaviorTree>();
    }

    private void Start()
    {
        _behaviorTree.SetVariableValue("PatrolPathPoints", patrolPath.Path);
    }


}
