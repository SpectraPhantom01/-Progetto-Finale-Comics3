using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorPlatform : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var mainMirrorPlatform = gameObject.GetComponentInParent<Main_MirrorPlatform>();

        if (mainMirrorPlatform != null)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawLine(transform.position, mainMirrorPlatform.gameObject.transform.position);
        }
    }
#endif   
}
