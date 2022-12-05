using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IAliveEntity
{
    public delegate void OnKillEnemy();
    public OnKillEnemy onKillEnemy;

    [SerializeField] GameObject damagerArea;

    BehaviorTree behaviorTree;

    public bool IsAlive { get; set; }
    public string Name { get ; set ; }

    private void Awake()
    {
        behaviorTree = GetComponent<BehaviorTree>();
        SetDamagerArea();
    }

    private void SetDamagerArea()
    {
        behaviorTree.SetVariableValue("DamagerArea", damagerArea);
    }

    private void Start()
    {
        if(GameManager.Instance.Player != null)
            Initialize("Target", GameManager.Instance.Player.gameObject);
    }

    private void Initialize(string targetBehaviorVariable, GameObject playerTarget)
    {
        IsAlive = true;
        Name = "Enemy_" + Guid.NewGuid().ToString();
        behaviorTree.SetVariableValue(targetBehaviorVariable, playerTarget);
    }

    public void SetFieldOfView(float newValue)
    {
        behaviorTree.SetVariableValue("FieldOfView", newValue);
    }

    public void SetFieldOfViewAngle(float newValue)
    {
        behaviorTree.SetVariableValue("FieldOfViewAngle", newValue);
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
}
