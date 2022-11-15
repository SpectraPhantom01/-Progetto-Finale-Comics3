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
    public Queue<Vector3> SavedPositions { get; private set; }

    public bool IsAlive { get ; set ; }
    public string Name { get ; set ; }

    private float savePositionTimePassed;
    private float hourglassTimePassed;
    private Damageable _damageable;
    public void Kill()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        IsAlive = true;
        Name = "Knight of Time";

        SavedPositions = new Queue<Vector3>();
        for (int i = 0; i < maxNumberPositions; i++)
        {
            SavedPositions.Enqueue(transform.position);
        }
        _damageable = gameObject.SearchComponent<Damageable>();
        savePositionTimePassed = 0;
    }

    private void Update()
    {
        HandleSavePosition();
        HandleHourglass();
    }

    private void HandleHourglass()
    {
        hourglassTimePassed += Time.deltaTime;
        if(hourglassTimePassed >= timeLoseDustInHourglass)
        {
            hourglassTimePassed = 0;
            _damageable.Damage(amountLoseDust);
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
    }
#endif
}
