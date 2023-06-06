using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoomTrigger : MonoBehaviour
{
    [HideInInspector] public List<EnemyController> EnemyControllers;
    [SerializeField] UnityEvent OnClearedRoom;
    private void Start()
    {
        foreach(EnemyController enemy in EnemyControllers)
        {
            enemy.onKillEnemy += () => { 
                EnemyControllers.Remove(enemy);
                if (EnemyControllers.Count == 0) 
                { OnClearedRoom.Invoke(); } 
            };
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
        if(playerManager != null)
        {
            EnemyControllers.ForEach(x => x.SetFieldOfView());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
        if (playerManager != null)
        {
            EnemyControllers.ForEach(x => x.ResetFieldOfView());
        }
    }
}
