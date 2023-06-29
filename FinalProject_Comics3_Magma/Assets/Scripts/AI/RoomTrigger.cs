using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class RoomTrigger : MonoBehaviour
{
    [HideInInspector] public List<EnemyController> EnemyControllers;
    [SerializeField] UnityEvent OnClearedRoom;
    [SerializeField] GameObject sfxToSpawnOnClearRoom;
    private void Start()
    {
        foreach (EnemyController enemy in EnemyControllers)
        {
            enemy.onKillEnemy += () =>
            {
                EnemyControllers.Remove(enemy);
                if (EnemyControllers.Count == 0)
                {

                    OnClearedRoom?.Invoke();

                }
            };
        }
    }

    public void SpawnSound()
    {
        if (sfxToSpawnOnClearRoom != null)
            Instantiate(sfxToSpawnOnClearRoom, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
        if (playerManager != null)
        {
            SetFieldOfView();
        }
    }

    public void SetFieldOfView()
    {
        foreach (var enemy in EnemyControllers.Where(e => e != null))
        {
            enemy.SetFieldOfView();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerManager playerManager = collision.GetComponentInParent<PlayerManager>();
        if (playerManager != null)
        {
            ResetFieldOfView();
        }
    }

    private void ResetFieldOfView()
    {
        foreach (var enemy in EnemyControllers.Where(e => e != null))
        {
            enemy.ResetFieldOfView();
        }
    }
}
