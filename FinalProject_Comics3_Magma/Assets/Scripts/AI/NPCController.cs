using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCController : AI, IAliveEntity
{

    public bool IsAlive { get; set; }
    public string Name { get; set; }
    public List<AttackScriptableObject> AttackList { get; }
    public UnityEvent OnKill;

    public void Kill()
    {
        OnKill.Invoke();

        if(SoundToSpawnOnKillPrefab != null)
            Instantiate(SoundToSpawnOnKillPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }


    public GameObject GetGameObject() => gameObject;

}
