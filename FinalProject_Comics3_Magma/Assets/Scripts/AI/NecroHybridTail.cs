using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecroHybridTail : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;
    [SerializeField] NecroHybrid necroHybrid;
    [SerializeField] float stunTime = 2f;
    [SerializeField] BehaviorTree behaviorTree;
    Damageable _damageable;
    bool stun;
    private void Start()
    {
        _damageable = gameObject.SearchComponent<Damageable>();

        _damageable.onGetDamage += Stun;
        
        if(enemyController.DestroyOnKill)
        {
            enemyController.onKillEnemy += () => Destroy(gameObject);
        }
        else
        {
            enemyController.onKillEnemy += () => gameObject.SetActive(false);
            enemyController.onReset += () => gameObject.SetActive(true);
            enemyController.onReset += () => gameObject.transform.position = enemyController.transform.position;
        }

        behaviorTree.SetVariableValue("Target", enemyController.gameObject);
    }

    private void Stun()
    {
        if (stun) return;

        stun = true;

        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        necroHybrid.SetVulnerable(true);
        enemyController.Damageable.Invincible = false;
        enemyController.BehaviorTree.SetVariableValue("Stun", true);
        enemyController.BehaviorTree.SetVariableValue("StunTime", stunTime);
        yield return new WaitForSeconds(stunTime);
        enemyController.Damageable.Invincible = true;
        stun = false;
        necroHybrid.SetVulnerable(false);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, enemyController.transform.position);
    }
#endif
}
