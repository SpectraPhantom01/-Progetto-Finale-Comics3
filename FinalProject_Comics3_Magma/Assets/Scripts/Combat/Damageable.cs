using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Damageable : MonoBehaviour
{
    [SerializeField] float _maxLife;
    private float _currentLife;
    private IAliveEntity _entity;
    private NavMeshAgent _agent;
    private BehaviorTree _behaviorTree;
    private Rigidbody2D _rb;

    [Range(0, 100)]
    [SerializeField] float KBResistance;

    private void Start()
    {
        _currentLife = _maxLife;

        _entity = gameObject.SearchComponent<IAliveEntity>();
        _agent = gameObject.SearchComponent<NavMeshAgent>();
        _behaviorTree = gameObject.SearchComponent<BehaviorTree>();
        _rb = gameObject.SearchComponent<Rigidbody2D>();
    }

    public void Damage(float amount, float KB, Vector2 direction)
    {
        _currentLife -= amount;
        Debug.Log($"Got damage!!! LIFE: {_currentLife}/{_maxLife}");


        if (_currentLife <= 0)
        {
            Debug.Log($"This damageable is death: {gameObject.name}");
            _entity.Kill();
        }
        else
        {
            StartCoroutine(KnockbackRoutine());
            float calculatedKB = CalculateKB(KB);
            _rb.AddForce(direction * calculatedKB, ForceMode2D.Impulse);
        }
    }

    private float CalculateKB(float KB)
    {
        KBResistance = (KBResistance / 100);
        KB = KB * (1 - KBResistance);
        return KB;
    }

    private IEnumerator KnockbackRoutine()
    {
        if (_agent != null)
            _agent.enabled = false;
        if (_behaviorTree != null)
            _behaviorTree.enabled = false;

        yield return new WaitForSeconds(0.5f);

        if (_agent != null)
            _agent.enabled = true;
        if (_behaviorTree != null)
            _behaviorTree.enabled = true;
    }

    public void Heal(float amount)
    {
        if (_currentLife + amount >= _maxLife)
        {
            _currentLife = _maxLife;
            Debug.Log($"This damageable has full life: {gameObject.name}");
            return;
        }

        _currentLife += amount;
        Debug.Log($"Got heal!!! LIFE: {_currentLife}/{_maxLife}");

    }

}
