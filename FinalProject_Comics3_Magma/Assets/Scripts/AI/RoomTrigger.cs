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
            foreach (var enemy in EnemyControllers)
            {
                switch (enemy.EnemyType)
                {
                    case EEnemyType.LavaSlime:
                        enemy.SetFieldOfView();
                        break;
                    case EEnemyType.DefensiveGolem:
                        enemy.SetFieldOfView();
                        break;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
        if (playerManager != null)
        {
            foreach (var enemy in EnemyControllers)
            {
                enemy.ResetFieldOfView();
            }
        }
    }
}
