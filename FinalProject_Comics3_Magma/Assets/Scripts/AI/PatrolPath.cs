using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{

    [HideInInspector] public List<GameObject> Path;

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Path.Add(transform.GetChild(i).gameObject);
        }
    }

#if UNITY_EDITOR
    [Header("Gizmo Settings")]
    [SerializeField] float gizmoSphereRadius = 0.5f;
    [SerializeField] Color lineColor;
    [SerializeField] Color sphereColor;
    private void OnDrawGizmos()
    {

        Gizmos.color = lineColor;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }

        Gizmos.color = sphereColor;
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.DrawSphere(transform.GetChild(i).position, gizmoSphereRadius);
        }

    }
#endif
}
