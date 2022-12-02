using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [SerializeField] float gizmoSphereRadius = 0.5f;

    [HideInInspector] public List<GameObject> Path;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Path.Add(transform.GetChild(i).gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.white;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.DrawSphere(transform.GetChild(i).position, gizmoSphereRadius);
        }

    }
#endif
}
