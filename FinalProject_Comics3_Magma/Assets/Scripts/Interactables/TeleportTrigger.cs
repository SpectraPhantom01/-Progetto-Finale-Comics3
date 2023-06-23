using UnityEngine;
using UnityEngine.Events;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] Transform destinationPoint;
    [SerializeField] UnityEvent onTeleportEvent;
    [Header("Read Tooltip")]
    [Tooltip("If Player is Ghosting you can revert actions by adding here a Teleport in Scene holding UnityEvents")]
    [SerializeField] TeleportTrigger listenerHolderToIgnore;
    [SerializeField] bool ignoreGhost;
    [SerializeField] GameObject vfxOnTeleportPrefab;
    public UnityEvent Listeners => onTeleportEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        {
            if (ignoreGhost && player.PlayerController.ImGhost)
                return;

            player.transform.position = destinationPoint.position;
            Instantiate(vfxOnTeleportPrefab, destinationPoint.position, Quaternion.identity);
            onTeleportEvent.Invoke();

            if (player.PlayerController.GhostActive && listenerHolderToIgnore != null)
            {
                player.PlayerController.Listeners = listenerHolderToIgnore.Listeners;
            }
        }
    }

    public void Switcher(GameObject gameObject)
    {
        if (gameObject.activeSelf)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    [SerializeField] bool activeGizmo = false;
    [SerializeField] Color lineColor = Color.white;
    private void OnDrawGizmos()
    {
        if (activeGizmo)
        {

            Gizmos.color = lineColor;
            Gizmos.DrawLine(transform.position, destinationPoint.position);
        }
    }
#endif
}
