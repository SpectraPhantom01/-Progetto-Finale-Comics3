using UnityEngine;
using UnityEngine.Events;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] Transform destinationPoint;
    [SerializeField] UnityEvent onTeleportEvent;
    [SerializeField] UnityEvent onTeleportHalfEvent;
    [SerializeField] UnityEvent onTeleportEndEvent;
    [Header("Read Tooltip")]
    [Tooltip("If Player is Ghosting you can revert actions by adding here a Teleport in Scene holding UnityEvents")]
    [SerializeField] TeleportTrigger listenerHolderToIgnore;
    [SerializeField] bool ignoreGhost;
    [SerializeField] GameObject vfxOnTeleportPrefab;
    [SerializeField, Range(0.5f, 5f)] float timeDelayTeleport;
    public UnityEvent Listeners => onTeleportEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        {
            if (ignoreGhost && player.PlayerController.ImGhost)
                return;

            player.Teleport(destinationPoint.position, timeDelayTeleport, vfxOnTeleportPrefab, onTeleportHalfEvent, onTeleportEndEvent);

            Instantiate(vfxOnTeleportPrefab, transform.position, Quaternion.identity);
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
