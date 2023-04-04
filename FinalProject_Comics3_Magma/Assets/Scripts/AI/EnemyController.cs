using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : AI, IAliveEntity
{
    public delegate void OnKillEnemy();
    public OnKillEnemy onKillEnemy;

    [Header("Settings")]
    [SerializeField] EEnemyType enemyType;
    [SerializeField] public float FieldOfViewAngle;
    [SerializeField] public float FieldOfViewAngleAfterSee;
    [SerializeField] public float FieldOfViewDistance;

    [Header("Attack types must be unique in this list")]
    [SerializeField] List<AttackScriptableObject> attackScriptableObjects;

    [Header("References")]
    [SerializeField] PatrolPath patrolPath;
    public EEnemyType EnemyType => enemyType;
    public bool IsAlive { get; set; }
    public string Name => GetName();
    public List<AttackScriptableObject> AttackList { get => attackScriptableObjects; }
    public bool DestroyOnKill = true;
    private string GetName()
    {
        return enemyType switch
        {
            EEnemyType.LavaSlime => "Lava Slime",
            EEnemyType.DefensiveGolem => "Defensive Golem",
            EEnemyType.BasicShootingEnemy => "Shooting Enemy",
            EEnemyType.Alchemic => "Alchemic Enemy",
            _ => Guid.NewGuid().ToString(),
        };
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

        if (DestroyOnKill)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    public GameObject GetGameObject() => gameObject;

#if UNITY_EDITOR
    [Header("Gizmo Settings")]
    [SerializeField] Color lineColor;
    [SerializeField] Color shootAttackRangeOfViewColor;
    private void OnDrawGizmos()
    {
        if (patrolPath != null && patrolPath.transform.GetChild(0) != null)
        {
            Gizmos.color = lineColor;
            Gizmos.DrawLine(transform.position, patrolPath.transform.GetChild(0).position);
        }

        var attackShoot = attackScriptableObjects.Find(x => x.AttackType == EAttackType.Shoot);
        if (attackShoot != null)
        {
            Gizmos.color = shootAttackRangeOfViewColor;
            Gizmos.DrawWireSphere(transform.position, attackShoot.shootAttackRangeOfView);
        }
    }
#endif 
}
