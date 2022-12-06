using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : AI, IAliveEntity
{
    [Header("Path")]
    [SerializeField] PatrolPath patrolPath;

    public bool IsAlive { get; set; }
    public string Name { get; set; }


    private void Start()
    {
        BehaviorTree.SetVariableValue("PatrolPathPoints", patrolPath.Path);
    }

    public void Kill()
    {
        Destroy(gameObject);
    }



#if UNITY_EDITOR
    [Header("Gizmo Settings")]
    [SerializeField] Color lineColor;

    private void OnDrawGizmos()
    {
        if (patrolPath == null) return;
        if (patrolPath.transform.GetChild(0) == null) return;

        Gizmos.color = lineColor;
        Gizmos.DrawLine(transform.position, patrolPath.transform.GetChild(0).position);

    }

#endif
}
