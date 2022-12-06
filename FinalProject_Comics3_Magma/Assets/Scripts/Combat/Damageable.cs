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
    private Hourglass _currentHourglass;

    private BehaviorTree _behaviorTree;
    private NavMeshAgent _agent;
    private Rigidbody2D _rigidBody;
    private AI ai;
    private PlayerManager playerManager;
    private bool _isPlayer;

    [Range(0, 100)]
    [SerializeField] float KnockBackResistance;

    private void Awake()
    {
        ai = gameObject.SearchComponent<AI>();
        _rigidBody = gameObject.SearchComponent<Rigidbody2D>();
        playerManager = gameObject.SearchComponent<PlayerManager>();
        if(ai != null)
        {
            _behaviorTree = ai.BehaviorTree;
            _agent = ai.Agent;
            _isPlayer = false;
        }
        else
            _isPlayer = true;
    }

    private void Start()
    {
        if (hourglasses == null || hourglasses.Count == 0)
            throw new ArgumentNullException($"La lista delle clessidre è vuota!!! {gameObject.name}");


        _currentHourglass = hourglasses.First();
        _currentTimeLife = _currentHourglass.Time;
        _entity = gameObject.SearchComponent<IAliveEntity>();

    }

    public void Damage(float amount, float knockBack, Vector2 direction)
    {
        _currentTimeLife -= amount;

        if (_currentTimeLife <= 0)
        {
            hourglasses.Remove(_currentHourglass);

            if(hourglasses.Count > 0)
                _currentHourglass = hourglasses.First();
            else
            {
                Debug.Log($"This damageable is death: {gameObject.name}");
                _entity.Kill();
                return;
            }

            _currentTimeLife = _currentHourglass.Time;
        }
        else
        {
            if(_isPlayer)
            {
                playerManager.LockMovement(0.5f);
            }
            else
            {
                StartCoroutine(KnockbackRoutine());
            }

            _rigidBody.AddForce(direction * CalculateKnockBack(knockBack), ForceMode2D.Impulse);
        }
    }

    private float CalculateKnockBack(float knockBack)
    {
        return knockBack * (1 - KnockBackResistance / 100);
    }

    private IEnumerator KnockbackRoutine()
    {
        if (_agent != null)
            _agent.enabled = false;
        if (_behaviorTree != null)
            _behaviorTree.enabled = false;

        _rigidBody.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(0.5f);

        if (_agent != null)
            _agent.enabled = true;
        if (_behaviorTree != null)
            _behaviorTree.enabled = true;

        _rigidBody.bodyType = RigidbodyType2D.Static;
    }

    public void Heal(float amount)
    {
        if (_currentTimeLife + amount >= _currentHourglass.Time)
        {
            _currentTimeLife = _currentHourglass.Time;
            Debug.Log($"This damageable has full TIME life: {gameObject.name}");
            return;
        }

        _currentTimeLife += amount;
        Debug.Log($"Got heal!!! TIME LIFE: {_currentTimeLife}/{_currentHourglass.Time}");

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
