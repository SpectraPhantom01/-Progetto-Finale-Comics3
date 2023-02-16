using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Damageable : MonoBehaviour
{
    [Header("Hourglasses Settings")]
    [SerializeField] List<Hourglass> hourglasses;
    [HideInInspector] public float PlayerLockTime = 0.1f;
    [HideInInspector] public float EnemyLockTime = 0.5f;
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
    private PlayerController playerController; //TEMPORANEO
    private bool _isPlayer;

    [Space(10)]

    [Header("Damageable Reductions")]
    [Range(0, 100)]
    [SerializeField] float KnockBackResistance;

    [Range(0, 100)]
    [SerializeField] float DamageReductionPerc;

    [SerializeField] float DamageReduction; //Da spostare in un eventuale SO e chiamarla Defence?
    

    private void Awake()
    {
        ai = gameObject.SearchComponent<AI>();
        _rigidBody = gameObject.SearchComponent<Rigidbody2D>();
        playerManager = gameObject.SearchComponent<PlayerManager>();
        playerController= gameObject.SearchComponent<PlayerController>();
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

    public void Damage(float amount, float knockBack, Vector2 direction, Transform respawnPosition = null)
    {
        CalculateDamage(amount);
        if(!_currentHourglass.Damage(25))
        {
            _currentTimeLife = -1;
        }

        if (_currentTimeLife <= 0)
        {
            hourglasses.Remove(_currentHourglass);

            if(hourglasses.Count > 0)
                _currentHourglass = hourglasses.First();
            else
            {
                if (respawnPosition != null)
                    _entity.Kill(respawnPosition.position);
                else
                    _entity.Kill(Vector3.zero);
                _currentTimeLife = 0;
                _currentHourglass = null;
                return;
            }

            if (respawnPosition != null)
                _entity.GetGameObject().transform.position = respawnPosition.position;

            _currentTimeLife = _currentHourglass.Time;
        }
        else
        {
            if(_isPlayer)
            {
                playerManager.LockMovement(PlayerLockTime);
                playerController.StateMachine.SetState(EPlayerState.Idle); //Introdurre uno stato di danneggiamento?

                ////TEMPORANEO
                ////-----------------------------------------------------------------
                //playerController.StopCoroutine(playerController.DashRoutine());
                //StartCoroutine(playerController.DashCooldownRoutine());
                ////-----------------------------------------------------------------
            }
            else
            {
                if(KnockBackResistance != 100)
                    StartCoroutine(KnockbackRoutine());
            }

            _rigidBody.AddForce(direction * CalculateKnockBack(knockBack), ForceMode2D.Impulse);
        }
    }

    private void CalculateDamage(float amount)
    {
        float damage = Mathf.Clamp(amount - DamageReduction, 0, 999);
        float actualDamage = damage * (1 - DamageReductionPerc / 100);

        _currentTimeLife -= actualDamage;
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

        yield return new WaitForSeconds(EnemyLockTime);

        if (_agent != null)
            _agent.enabled = true;
        if (_behaviorTree != null)
            _behaviorTree.enabled = true;

        _rigidBody.bodyType = RigidbodyType2D.Static;
    }

    public void Heal(float amount)
    {
        if(_currentHourglass == null)
        {

            hourglasses.Add(new Hourglass(amount));
            _currentHourglass = hourglasses[0];
            _currentTimeLife = amount;
            return;
        }

        if (_currentTimeLife + amount >= _currentHourglass.Time)
        {
            _currentTimeLife = _currentHourglass.Time;
            Debug.Log($"This damageable has full TIME life: {gameObject.name}");
            return;
        }

        _currentTimeLife += amount;
        Debug.Log($"Got heal!!! TIME LIFE: {_currentTimeLife}/{_currentHourglass.Time}");

    }

    public void HealHourglass(float percentage)
    {
        _currentHourglass.Heal(percentage);
    }

    public void SetHourglasses(List<Hourglass> newHourglasses)
    {
        hourglasses = newHourglasses;
    }

    public void SetCurrentLife(float amount)
    {
        _currentTimeLife = amount;
    }


    private void OnValidate()
    {
        DamageReduction = Mathf.Clamp(DamageReduction, 0, float.MaxValue);
    }

}
