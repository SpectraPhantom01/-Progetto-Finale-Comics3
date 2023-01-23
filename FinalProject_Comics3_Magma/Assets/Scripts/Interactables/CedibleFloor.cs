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
            else if(gameObject.layer == killZoneLayer)
            {
                player.Damageable.Damage(damageAmount, knockBackAmount, knockBackDirection);
            }

        }
    }

    private IEnumerator SwapFloorCoroutine(float timeBeforeSwapFloor)
    {
        yield return new WaitForSeconds(timeBeforeSwapFloor);
        normalFloor.SetActive(false);
        lavaFloor.SetActive(true);
        gameObject.layer = killZoneLayer;
        var collider = GetComponent<BoxCollider2D>();
        collider.enabled = false;
        yield return new WaitForEndOfFrame();
        collider.enabled = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (gameObject.layer == killZoneLayer)
        {
            var player = collision.gameObject.SearchComponent<PlayerManager>();
            if (player != null)
            {
                player.Damageable.Damage(damageAmount, knockBackAmount, knockBackDirection);

            }
        }
    }
}
