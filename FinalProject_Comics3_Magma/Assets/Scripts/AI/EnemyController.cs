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
    [SerializeField] public float FieldOfViewAngle;
    [SerializeField] public float FieldOfViewAngleAfterSee;
    [SerializeField] public float FieldOfViewDistance;
    [SerializeField] List<AttackScriptableObject> attackScriptableObjects;

    [Header("References")]
    [SerializeField] PatrolPath patrolPath;

    public EEnemyType EnemyType => enemyType;
    public bool IsAlive { get; set; }
    public string Name => GetName();

    public List<AttackScriptableObject> AttackList { get => attackScriptableObjects; }

    private string GetName()
    {
        switch (enemyType)
        {
            case EEnemyType.LavaSlime:
                return "Lava Slime";
            case EEnemyType.DefensiveGolem:
                return "Defensive Golem";
            default:
                return Guid.NewGuid().ToString();
        }
    }

    private Damager _damager;

    private void Start()
    {
        _damager = gameObject.SearchComponent<Damager>();

        if (GameManager.Instance.Player != null)
            Initialize("Target", GameManager.Instance.Player.gameObject);

    }

    private void Initialize(string targetBehaviorVariable, GameObject playerTarget)
    {
        IsAlive = true;
        BehaviorTree.SetVariableValue(targetBehaviorVariable, playerTarget);
        BehaviorTree.SetVariableValue("Damager", _damager.gameObject);
        switch (enemyType)
        {
            case EEnemyType.LavaSlime:
                break;
            case EEnemyType.DefensiveGolem:
                BehaviorTree.SetVariableValue("PatrolPathPoints", patrolPath.Path);
                break;
        }
    }

    public void SetFieldOfView()
    {
        BehaviorTree.SetVariableValue("FieldOfView", FieldOfViewDistance);
    }

    public void ResetFieldOfView()
    {
        BehaviorTree.SetVariableValue("FieldOfView", 0);
    }

    public void SetFieldOfViewAngle(float newValue)
    {
        BehaviorTree.SetVariableValue("FieldOfViewAngle", newValue);
    }


    public void Kill()
    {
        onKillEnemy?.Invoke();

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
