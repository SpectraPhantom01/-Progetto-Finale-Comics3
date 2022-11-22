using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [SerializeField] List<EnemyController> _enemyControllers;
    [SerializeField] float _fieldOfView;
    [SerializeField] float _fieldOfViewAngleReset;
    [SerializeField] float _fieldOfViewAngleOnSee;

    private void Start()
    {
        foreach(EnemyController enemy in _enemyControllers)
        {
            enemy.onKillEnemy += () => _enemyControllers.Remove(enemy);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
        if(playerManager != null)
        {
            foreach (var enemy in _enemyControllers)
            {
                enemy.SetFieldOfView(_fieldOfView);
                enemy.SetFieldOfViewAngle(_fieldOfViewAngleOnSee);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
        if (playerManager != null)
        {
            foreach (var enemy in _enemyControllers)
            {
                enemy.SetFieldOfView(0);
            }
        }
    }
}
