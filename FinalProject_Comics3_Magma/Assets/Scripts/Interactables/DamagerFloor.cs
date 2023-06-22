using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagerFloor : MonoBehaviour
{
    [SerializeField] float damageAmount;
    [SerializeField] float hourglassPercentageDamageAmount;
    [SerializeField] Transform respawnPoint;
    [SerializeField] AudioSource audioSourceOnCollision;
    public Transform RespawnPoint => respawnPoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        { 
            if (!player.PlayerController.IsDashing)
            {
                audioSourceOnCollision.Play();

                player.Damageable.Damage(damageAmount, 0, Vector2.zero, hourglassPercentageDamageAmount);
                if (respawnPoint != null)
                    player.Respawn(respawnPoint.position);
                else
                    player.RespawnToCheckpoint();

            }

        }
        else
        {
            var enemy = collision.gameObject.SearchComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Kill();
            }
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null && !player.PlayerController.IsDashing)
        {
            player.Damageable.Damage(damageAmount, 0, Vector2.zero, hourglassPercentageDamageAmount);

            audioSourceOnCollision.Play();

            if (respawnPoint != null)
                player.Respawn(respawnPoint.position);
            else
                player.RespawnToCheckpoint();
        }
        else
        {
            var enemy = collision.gameObject.SearchComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.Kill();
            }
        }
    }

    public void SetRespawnpoint(Transform newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var cedibleArea = gameObject.GetComponentInParent<CedibleFloorArea>();
        if (cedibleArea != null)
            respawnPoint = cedibleArea.RespawnPoint;
        if (respawnPoint != null)
        {
            Gizmos.color = Color.white;

            Gizmos.DrawLine(transform.position, respawnPoint.position);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(respawnPoint.position, 0.1f);
        }
    }
#endif
}
