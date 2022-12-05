using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Damageable : MonoBehaviour
{
    [SerializeField] List<Hourglass> hourglasses;

    public float CurrentTimeLife => _currentTimeLife;
    public int Hourglasses => hourglasses.Count;

    private float _currentTimeLife;
    private IAliveEntity _entity;
    private Hourglass currentHourglass;

    private BehaviorTree _behaviorTree;
    private NavMeshAgent _agent;
    private Rigidbody2D _rb;

    [Range(0, 100)]
    [SerializeField] float KBResistance;

    private void Start()
    {
        if (hourglasses == null || hourglasses.Count == 0)
            throw new ArgumentNullException($"La lista delle clessidre è vuota!!! {gameObject.name}");


        currentHourglass = hourglasses.First();
        _currentTimeLife = currentHourglass.Time;
        _entity = gameObject.SearchComponent<IAliveEntity>();

        _behaviorTree = gameObject.SearchComponent<BehaviorTree>();
        _agent = gameObject.SearchComponent<NavMeshAgent>();
        _rb = gameObject.SearchComponent<Rigidbody2D>();
    }

    public void Damage(float amount, float KB, Vector2 direction)
    {
        _currentTimeLife -= amount;
        Debug.Log($"Got damage!!! TIME LIFE: {_currentTimeLife}/{currentHourglass.Time}");

        if (_currentTimeLife <= 0)
        {
            hourglasses.Remove(currentHourglass);

            currentHourglass = hourglasses.First();
            if(currentHourglass == null)
            {
                Debug.Log($"This damageable is death: {gameObject.name}");
                _entity.Kill();
                return;
            }

            _currentTimeLife = currentHourglass.Time;
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
        if (_currentTimeLife + amount >= currentHourglass.Time)
        {
            _currentTimeLife = currentHourglass.Time;
            Debug.Log($"This damageable has full TIME life: {gameObject.name}");
            return;
        }

        _currentTimeLife += amount;
        Debug.Log($"Got heal!!! TIME LIFE: {_currentTimeLife}/{currentHourglass.Time}");

    }

    public void SetHourglasses(List<Hourglass> newHourglasses)
    {
        hourglasses = newHourglasses;
    }

    public void SetCurrentLife(float amount)
    {
        _currentTimeLife = amount;
    }
}
