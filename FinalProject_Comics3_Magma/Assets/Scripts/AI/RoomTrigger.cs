using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomTrigger : MonoBehaviour
{
    [HideInInspector] public List<EnemyController> EnemyControllers;
    [SerializeField] UnityEvent OnClearedRoom;
    private void Start()
    {
        foreach (EnemyController enemy in EnemyControllers)
        {
            enemy.onKillEnemy += () =>
            {
                EnemyControllers.Remove(enemy);
                if (EnemyControllers.Count == 0)
                { OnClearedRoom.Invoke(); }
            };
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
        if (playerManager != null)
        {
            foreach (var enemy in EnemyControllers.Where(e => e != null))
            {
                enemy.SetFieldOfView();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
        if (playerManager != null)
        {
            foreach (var enemy in EnemyControllers.Where(e => e != null))
            {
                enemy.ResetFieldOfView();
            }
        }
    }
}
