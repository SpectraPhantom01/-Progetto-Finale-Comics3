using BehaviorDesigner.Runtime;
using Spine.Unity;
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
    [HideInInspector] public float PlayerLockTime = 0.3f;
    [HideInInspector] public float LockTime = 0.5f;
    public float CurrentTimeLife => _currentTimeLife;
    public int HourglassesCount => hourglasses.Count;
    public bool Invincible = false;
    private float _currentTimeLife;
    private IAliveEntity _entity;
    public List<Hourglass> Hourglasses { get { return hourglasses; } }
    private Hourglass _currentHourglass;

    private BehaviorTree _behaviorTree;
    private NavMeshAgent _agent;
    private Rigidbody2D _rigidBody;
    private AI ai;
    private PlayerManager playerManager;
    private PlayerController playerController; //TEMPORANEO
    private bool _isPlayer;

    public delegate void OnGetDamage();
    public OnGetDamage onGetDamage;

    [Space(10)]

    [Header("Damageable Reductions")]
    [Range(0, 100)]
    [SerializeField] float KnockBackResistance;

    [Range(0, 100)]
    [SerializeField] float DamageReductionPerc;

    [SerializeField] float DamageReduction; //Da spostare in un eventuale SO e chiamarla Defence?
    [Header("VFX OnDamage")]
    [SerializeField] GameObject vfxToSpawnOnDamage;

    public Hourglass CurrentHourglass => _currentHourglass;

    private Coroutine resistanceCoroutine;
    private Coroutine materialDamageCoroutine;
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
        else if(playerManager != null || playerController != null)
            _isPlayer = true;
    }

    private void Start()
    {
        if (hourglasses == null || hourglasses.Count == 0)
            throw new ArgumentNullException($"La lista delle clessidre è vuota!!! {gameObject.name}");


        _currentHourglass = hourglasses.First();
        _currentTimeLife = _currentHourglass.Time;

        _entity ??= gameObject.SearchComponent<IAliveEntity>();

    }

    public void Initialize(IAliveEntity entity, Rigidbody2D rigidbody)
    {
        _entity = entity;
        _rigidBody = rigidbody;
    }

    public void Damage(float amount, float knockBack, Vector2 direction, float hourglassPercentageDamage)
    {
        onGetDamage?.Invoke();

        if (Invincible)
            return;

        SpawnDamageVFX();

        CalculateDamage(amount);
        if (_currentHourglass != null)
            _currentHourglass.Damage(hourglassPercentageDamage);
        else
            return;

        if (_currentTimeLife <= 0)
        {
            hourglasses.Remove(_currentHourglass);

            if (hourglasses.Count > 0)
                UseNext();
            else
            {
                _entity.Kill();
                _currentTimeLife = 0;
                _currentHourglass = null;
                return;
            }

            _currentTimeLife = _currentHourglass.Time;
        }
        else
        {
            if(_isPlayer)
            {
                StartCoroutine(KnockbackRoutine(PlayerLockTime));
                playerController.StateMachine.SetState(EPlayerState.Idle); 
            }
            else
            {
                if(KnockBackResistance != 100)
                    StartCoroutine(KnockbackRoutine(LockTime));
            }

            if(direction != Vector2.zero)
                _rigidBody.AddForce(direction * CalculateKnockBack(knockBack), ForceMode2D.Impulse);
        }
    }

    private void UseNext()
    {
        _currentHourglass = hourglasses.First();
        if(_isPlayer)
        {
            Publisher.Publish(new UseNextHourglassMessage());
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

    private IEnumerator KnockbackRoutine(float time)
    {
        if (_agent != null)
        {
            _agent.SetDestination(_agent.gameObject.transform.position);
            _agent.velocity = Vector3.zero;
            _agent.isStopped = true;
        }
        if (_behaviorTree != null)
            _behaviorTree.enabled = false;

        if(_isPlayer)
        {
            playerController.CanMove = false;
        }
        else
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;

        yield return new WaitForSeconds(time);
        
        if (_behaviorTree != null)
            _behaviorTree.enabled = true;

        if (_isPlayer)
        {
            playerController.CanMove = true;
        }
        else
            _rigidBody.bodyType = RigidbodyType2D.Static;
    }

    public void Heal(float amount)
    {
        if(_currentHourglass == null)
        {

            hourglasses.Add(new Hourglass(amount));
            _currentHourglass = hourglasses[0];
            _currentTimeLife = amount;
            Publisher.Publish(new AddNewHourglass());
            return;
        }

        if (_currentTimeLife + amount >= _currentHourglass.Time)
        {
            _currentTimeLife = _currentHourglass.Time;
            return;
        }

        _currentTimeLife += amount;

    }

    public void HealHourglass(float percentage)
    {
        _currentHourglass.Heal(percentage);
    }

    public void SetHourglasses(List<Hourglass> newHourglasses)
    {
        hourglasses = newHourglasses.ToList();
    }

    public void SetCurrentLife(float amount)
    {
        _currentTimeLife = amount;
    }


    private void OnValidate()
    {
        DamageReduction = Mathf.Clamp(DamageReduction, 0, float.MaxValue);
    }

    public void Damage(float amount)
    {
        if(_isPlayer)
        {
            CalculateDamage(amount);

            if (_currentTimeLife <= 0)
            {
                hourglasses.Remove(_currentHourglass);

                if (hourglasses.Count > 0)
                    UseNext();
                else
                {
                    _entity.Kill();
                    _currentTimeLife = 0;
                    _currentHourglass = null;
                    return;
                }

                _currentTimeLife = _currentHourglass.Time;
            }
        }
    }

    internal void StartNewResistance(float effectInPercentage, float effectInTime)
    {
        if(resistanceCoroutine == null)
        {
            resistanceCoroutine = StartCoroutine(ResistanceCoroutine(effectInPercentage, effectInTime));
        }
        else
        {
            StopCoroutine(resistanceCoroutine);
            resistanceCoroutine = StartCoroutine(ResistanceCoroutine(effectInPercentage, effectInTime));
        }
    }

    private IEnumerator ResistanceCoroutine(float effectInPercentage, float effectInTime)
    {
        float castKnockBack = KnockBackResistance;
        float castDamageReductionPerc = DamageReductionPerc;
        float castDamageReduction = DamageReduction;

        KnockBackResistance += castKnockBack * (effectInPercentage / 100);
        DamageReductionPerc += castDamageReductionPerc * (effectInPercentage / 100);
        DamageReduction += castDamageReduction * (effectInPercentage / 100);


        yield return new WaitForSeconds(effectInTime);
        KnockBackResistance = castKnockBack;
        DamageReductionPerc = castDamageReductionPerc;
        DamageReduction = castDamageReduction;
    }

    public void SpawnDamageVFX()
    {
        Instantiate(vfxToSpawnOnDamage, transform.position, Quaternion.identity);
    }

    public float GetTotalLifeTime()
    {
        float lifeTime = 0;
        foreach (var hourglass in Hourglasses)
        {
            lifeTime += hourglass.Time;
        }
        return lifeTime;
    }

    public float GetRelativeTotalLifeTime()
    {
        float lifeTime = CurrentTimeLife;
        for (int i = 1; i < Hourglasses.Count; i++)
        {
            Hourglass hourglass = Hourglasses[i];
            lifeTime += hourglass.Time;
        }
        return lifeTime;
    }
}
