using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class RoomManager : MonoBehaviour
{
    [SerializeField] LayerMask collisionMask;

    [Header("Spawn Manager")]
    [Tooltip("Relazione 1 a 1 tra prefab to spawn e position to spawn")]
    [SerializeField] List<EnemyController> prefabToSpawn;
    [Tooltip("Relazione 1 a 1 tra prefab to spawn e position to spawn")]
    [SerializeField] List<Transform> positionToSpawn;
    [SerializeField] List<int> spawnedObjectCount;

    Queue<EnemyController> spawnedQueue;
    List<EnemyController> spawnedObjects;
    RoomTrigger roomTrigger;
    private void Awake()
    {
        spawnedQueue = new Queue<EnemyController>();
        spawnedObjects = new List<EnemyController>();
        roomTrigger = GetComponentInChildren<RoomTrigger>();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collisionMask.Contains(collision.gameObject.layer))
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if(spawnedQueue.Count > 0)
        {
            int count = spawnedQueue.Count;
            for (int i = 0; i < count; i++)
            {
                var objectSpawned = spawnedQueue.Dequeue();
                objectSpawned.gameObject.SetActive(true);
                objectSpawned.transform.position = positionToSpawn[Random.Range(0, positionToSpawn.Count)].position;
            }
        }
        else
        {
            
            for (int i = 0; i < spawnedObjectCount.Count; i++)
            {
                for (int j = 0; j < spawnedObjectCount[i]; j++)
                {
                    var newObject = Instantiate(prefabToSpawn[i]);
                    if (i < positionToSpawn.Count)
                        newObject.transform.position = positionToSpawn[i].position;
                    else
                        newObject.transform.position = positionToSpawn[0].position;

                    newObject.DestroyOnKill = false;
                    newObject.onKillEnemy += () => spawnedQueue.Enqueue(newObject);
                    spawnedObjects.Add(newObject);

                    roomTrigger.EnemyControllers.Add(newObject);

                    newObject.Stop();
                    Invoke(nameof(DelayInvoke), 0.2f);
                }
            }
        }
        
    }

    private void DelayInvoke()
    {
        spawnedObjects.ForEach(x => x.Play());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collisionMask.Contains(collision.gameObject.layer))
        {
            foreach (var enemy in spawnedObjects)
            {
                if(enemy.gameObject.activeSelf)
                {
                    spawnedQueue.Enqueue(enemy);
                    enemy.gameObject.SetActive(false);
                }
            }
        }
    }

}
