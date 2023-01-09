using UnityEngine;
using UnityEngine.Events;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] Transform destinationPoint;
    [SerializeField] UnityEvent onTeleportEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (player != null)
        {
            player.transform.position = destinationPoint.position;
            onTeleportEvent.Invoke();
        }
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
