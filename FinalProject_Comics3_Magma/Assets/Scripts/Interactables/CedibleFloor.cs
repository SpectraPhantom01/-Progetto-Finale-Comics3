using System.Collections;
using UnityEngine;

public class CedibleFloor : MonoBehaviour
{
    [SerializeField] int killZoneLayer;
    [SerializeField] GameObject normalFloor;
    [SerializeField] GameObject lavaFloor;
    [SerializeField] float timeBeforeSwapFloor;
    [SerializeField] float damageAmount;
    [SerializeField] float knockBackAmount;
    [SerializeField] Vector2 knockBackDirection;
    [SerializeField] ParticleSystem cedibleFloorVFX;
    [SerializeField] Transform respawnPoint;
    bool swapped;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        {
            if (!swapped)
            {
                var particle = Instantiate(cedibleFloorVFX, transform.position, Quaternion.identity);
                Destroy(particle, 1f);
                StartCoroutine(SwapFloorCoroutine(timeBeforeSwapFloor));
                swapped = true;
            }
             else if(gameObject.layer == killZoneLayer && !player.PlayerController.IsDashing)
            {
                player.Damageable.Damage(damageAmount, knockBackAmount, knockBackDirection);
                player.Respawn(respawnPoint.position);
            }

        }
    }

    private IEnumerator SwapFloorCoroutine(float timeBeforeSwapFloor)
    {
        var collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
        yield return new WaitForSeconds(timeBeforeSwapFloor);
        normalFloor.SetActive(false);
        lavaFloor.SetActive(true);
        gameObject.layer = killZoneLayer;
        collider.enabled = true;
    }

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (gameObject.layer == killZoneLayer)
    //    {
    //        var player = collision.gameObject.SearchComponent<PlayerManager>();
    //        if (player != null && !player.PlayerController.IsDashing)
    //        {
    //            player.Damageable.Damage(damageAmount, knockBackAmount, knockBackDirection);

    //            player.Respawn(respawnPoint.position);
    //        }
    //    }
    //}
}
