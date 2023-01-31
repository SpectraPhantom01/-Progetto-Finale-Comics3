using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PlayerManager : MonoBehaviour, IAliveEntity
{
    [Header("Time Machine Settings")]
    [SerializeField] int maxNumberPositions;
    [SerializeField] float timeDeltaSavePosition;
    [Header("Hourglass Settings")]
    [SerializeField] float timeLoseDustInHourglass;
    [SerializeField] float amountLoseDust;
    [Header("Attack Settings")]
    [SerializeField] List<AttackScriptableObject> attackScriptableObjects;
    public Queue<Vector3> SavedPositions { get; private set; }

    public EDirection CurrentDirection = EDirection.Down;
    public bool IsAlive { get ; set ; }
    public string Name => "Knight of Time";
    public Damageable Damageable => _damageable;

    public List<AttackScriptableObject> AttackList { get => attackScriptableObjects; }
    public PlayerController PlayerController => _playerController;
    private float savePositionTimePassed;
    private float hourglassTimePassed;
    private Damageable _damageable;
    private PlayerController _playerController;

    public void Kill(Vector3 respawnPosition)
    {

        Debug.Log("animazione SEI MORTO!");
        StartCoroutine(KillCoroutine(respawnPosition));
    }

    public void Respawn(Vector3 position)
    {
        transform.position = position;
    }

    private IEnumerator KillCoroutine(Vector3 respawnPosition)
    {
        yield return new WaitForSeconds(1);

        transform.position = respawnPosition;
        Damageable.Heal(50);
    }

    private void Awake()
    {
        IsAlive = true;

        SavedPositions = new Queue<Vector3>();
        for (int i = 0; i < maxNumberPositions; i++)
        {
            SavedPositions.Enqueue(transform.position);
        }
        _damageable = gameObject.SearchComponent<Damageable>();
        savePositionTimePassed = 0;

        _playerController = gameObject.SearchComponent<PlayerController>();
    }

    private void Update()
    {
        HandleSavePosition();
        HandleHourglass();

        if(_playerController.Rigidbody.velocity.magnitude > 0.01f)
            CurrentDirection = _playerController.Rigidbody.velocity.CalculateDirection();
    }

    private void HandleHourglass()
    {
        hourglassTimePassed += Time.deltaTime;
        if(hourglassTimePassed >= timeLoseDustInHourglass)
        {
            hourglassTimePassed = 0;
            //_damageable.Damage(amountLoseDust);
        }
    }

    private void HandleSavePosition()
    {
        savePositionTimePassed += Time.deltaTime;
        if(savePositionTimePassed >= timeDeltaSavePosition)
        {
            savePositionTimePassed = 0;
            SavePosition(transform.position);
        }
    }

    private void SavePosition(Vector3 newPosition)
    {
        SavedPositions.Dequeue();
        SavedPositions.Enqueue(newPosition);
    }

    public void LockMovement(float time)
    {
        if(_playerController.CanMove)
            StartCoroutine(LockCoroutine(time));
            
    }

    private IEnumerator LockCoroutine(float time)
    {
        _playerController.CanMove = false;
        yield return new WaitForSeconds(time);
        _playerController.CanMove = true;
    }

    public GameObject GetGameObject() => gameObject;
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(SavedPositions != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var savedPos in SavedPositions)
            {
                Gizmos.DrawSphere(savedPos, 0.5f);
            }
            Gizmos.color = Color.white;
            var positions = SavedPositions.ToArray();
            for (int i = 0; i < positions.Length - 1; i++)
            {
                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }
        }

        var attackShoot = attackScriptableObjects.Find(x => x.AttackType == EAttackType.Shoot);
        if (attackShoot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackShoot.shootAttackRangeOfView);
        }
    }

#endif
}
