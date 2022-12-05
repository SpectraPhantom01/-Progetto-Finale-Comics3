using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : AI, IAliveEntity
{
    public delegate void OnKillEnemy();
    public OnKillEnemy onKillEnemy;

    [Header("Settings")]
    [SerializeField] EEnemyType enemyType;

    [Header("References")]
    [SerializeField] GameObject damagerArea;
    [SerializeField] PatrolPath patrolPath;

    public GameObject DamagerArea => damagerArea;
    public bool IsAlive { get; set; }
    public string Name { get ; set ; }

    private void SetDamagerArea()
    {
        BehaviorTree.SetVariableValue("DamagerArea", damagerArea);
    }

    private void Start()
    {
        if(GameManager.Instance.Player != null)
            Initialize("Target", GameManager.Instance.Player.gameObject);
        SetDamagerArea();
    }

    private void Initialize(string targetBehaviorVariable, GameObject playerTarget)
    {
        IsAlive = true;
        Name = "Enemy_" + Guid.NewGuid().ToString();
        BehaviorTree.SetVariableValue(targetBehaviorVariable, playerTarget);

        switch (enemyType)
        {
            case EEnemyType.LavaSlime:
                break;
            case EEnemyType.DefensiveGolem:
                BehaviorTree.SetVariableValue("PatrolPathPoints", patrolPath.Path);
                break;
        }
    }

    public void SetFieldOfView(float newValue)
    {
        BehaviorTree.SetVariableValue("FieldOfView", newValue);
    }

    public void SetFieldOfViewAngle(float newValue)
    {
        BehaviorTree.SetVariableValue("FieldOfViewAngle", newValue);
    }

    public void SetActiveDamagerArea(bool active)
    {
        damagerArea.SetActive(active);
    }

    public void Kill()
    {
        onKillEnemy.Invoke();

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
