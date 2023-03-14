using System.Collections;
using UnityEngine;

public class CedibleFloor : MonoBehaviour
{
    [SerializeField] GameObject normalFloor;
    [SerializeField] GameObject damagedFloor;
    [SerializeField] DamagerFloor lavaFloor;
    [SerializeField] float timeBeforeSwapFloor;
    [SerializeField] ParticleSystem cedibleFloorVFX;
    [SerializeField] Collider2D lavaCollider;
    private void Awake()
    {
        var cedibleArea = gameObject.GetComponentInParent<CedibleFloorArea>();
        if (cedibleArea != null)
        {
            lavaFloor.SetRespawnpoint(cedibleArea.RespawnPoint);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        {
            var particle = Instantiate(cedibleFloorVFX, transform.position, Quaternion.identity);
            Destroy(particle.gameObject, 1f);
            StartCoroutine(SwapFloorCoroutine(timeBeforeSwapFloor));
        }
    }

    private IEnumerator SwapFloorCoroutine(float timeBeforeSwapFloor)
    {
        damagedFloor.SetActive(true);
        normalFloor.SetActive(false);
        Destroy(GetComponent<BoxCollider2D>());
        yield return new WaitForSeconds(timeBeforeSwapFloor);
        damagedFloor.SetActive(false);
        lavaCollider.enabled = true;
    }


}
