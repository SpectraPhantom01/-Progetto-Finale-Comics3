using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : AI, IAliveEntity
{
    public delegate void OnKillEnemy();
    public OnKillEnemy onKillEnemy;
    public delegate void OnResetBehavior();
    public OnResetBehavior onReset;

    [Header("Settings")]
    [SerializeField] EEnemyType enemyType;
    [SerializeField] public float FieldOfViewAngle;
    [SerializeField] public float FieldOfViewAngleAfterSee;
    [SerializeField] public float FieldOfViewDistance;
    [SerializeField] Transform cannonShootArea;

    [Header("Attack types must be unique in this list")]
    [SerializeField] List<AttackScriptableObject> attackScriptableObjects;

    [Header("References")]
    [SerializeField] PatrolPath patrolPath;
    [SerializeField] GameObject shootingEnemyGraphicsPrefab;
    [SerializeField] Vector3 spawnGraphicsOffset;
    [SerializeField] bool spawnRotated;
    [SerializeField] Vector2 boxColliderOffset;

    [Header("Event On Kill")]
    [SerializeField] UnityEvent onKillEnemySceneRef;

    public UnityEvent OnKillEnemySceneRef => onKillEnemySceneRef;
    public EEnemyType EnemyType => enemyType;
    public bool IsAlive { get; set; }
    public string Name => GetName();
    public List<AttackScriptableObject> AttackList { get => attackScriptableObjects; }
    public bool DestroyOnKill = true;
    private List<Hourglass> initialHourglasses;
    private GameObject shootingEnemyGraphics;
    private string GetName()
    {
        return enemyType switch
        {
            EEnemyType.LavaSlime => "Lava Slime",
            EEnemyType.DefensiveGolem => "Defensive Golem",
            EEnemyType.BasicShootingEnemy => "Shooting Enemy",
            EEnemyType.Alchemic => "Alchemic Enemy",
            EEnemyType.NecroHybrid => "Necro Hybrid",
            EEnemyType.LoaderPuncher => "Loader Puncher",
            EEnemyType.MouthOfEmpty => "Mouth of Empty",
            EEnemyType.Boss => "Boss ???",
            _ => Guid.NewGuid().ToString(),
        };
    }


    private void Start()
    {
        initialHourglasses = new List<Hourglass>();

        if (enemyType == EEnemyType.BasicShootingEnemy)
        {
            shootingEnemyGraphics = Instantiate(shootingEnemyGraphicsPrefab, transform.position, spawnRotated ? Quaternion.Euler(0, 180, 0) : Quaternion.identity);
            
            Damager = shootingEnemyGraphics.GetComponentInChildren<Damager>();
            Damager.Initialize(AttackList, cannonShootArea);

            Damageable = shootingEnemyGraphics.GetComponentInChildren<Damageable>();
            Damageable.Initialize(this, GetComponent<Rigidbody2D>());

            shootingEnemyGraphics.transform.position += spawnGraphicsOffset;
            onKillEnemy += () => Destroy(shootingEnemyGraphics);
        }


        if (GameManager.Instance.Player != null)
            Initialize("Target", GameManager.Instance.Player.gameObject);

        foreach (Hourglass hourglass in Damageable.Hourglasses)
        {
            initialHourglasses.Add(new Hourglass(hourglass.Time)
            {
                HourglassLife = hourglass.HourglassLife,
                BaseTimeLoseSand = hourglass.BaseTimeLoseSand,
                MaxSpeedLoseSand = hourglass.MaxSpeedLoseSand,
            });
        }
    }

    public virtual void Initialize(string targetBehaviorVariable, GameObject playerTarget)
    {
        IsAlive = true;
        BehaviorTree.SetVariableValue(targetBehaviorVariable, playerTarget);
        BehaviorTree.SetVariableValue("Damager", Damager.gameObject);
        switch (enemyType)
        {
            case EEnemyType.DefensiveGolem:
                BehaviorTree.SetVariableValue("PatrolPathPoints", patrolPath.Path);
                break;
        }
    }

    public void SetFieldOfView()
    {
        if(BehaviorTree != null)
            BehaviorTree.SetVariableValue("FieldOfView", FieldOfViewDistance);
    }

    public void ResetFieldOfView()
    {
        if (BehaviorTree != null)
            BehaviorTree.SetVariableValue("FieldOfView", 0);
    }

    public void SetFieldOfViewAngle(float newValue)
    {
        if (BehaviorTree != null)
            BehaviorTree.SetVariableValue("FieldOfViewAngle", newValue);
    }


    public virtual void Kill()
    {
        onKillEnemy?.Invoke();
        onKillEnemySceneRef.Invoke();

        if(SoundToSpawnOnKillPrefab != null)
            Destroy(Instantiate(SoundToSpawnOnKillPrefab, transform.position, Quaternion.identity), 1.5f);

        if (DestroyOnKill)
            Destroy(gameObject);
    }

    public GameObject GetGameObject() => gameObject;

    public void ResetBehavior()
    {
        onReset.Invoke();
        Damageable.SetHourglasses(initialHourglasses);
    }


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
