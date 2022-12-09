using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField ] public List<EnemyController> EnemyControllers;

    private void Start()
    {
        foreach(EnemyController enemy in EnemyControllers)
        {
            enemy.onKillEnemy += () => EnemyControllers.Remove(enemy);
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
