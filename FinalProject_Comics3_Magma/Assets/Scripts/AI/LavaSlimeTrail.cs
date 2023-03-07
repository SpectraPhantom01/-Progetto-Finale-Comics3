using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSlimeTrail : MonoBehaviour
{
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] float colliderEdgeRadius;
    [SerializeField] EdgeCollider2D trailCollider;
    EdgeCollider2D edgeCollider;

    private void Awake()
    {
        edgeCollider = Instantiate(trailCollider);
        edgeCollider.edgeRadius = colliderEdgeRadius;
    }

    private void Update()
    {
        SetCollidersPoints();
    }

    private void SetCollidersPoints()
    {
        List<Vector2> points = new List<Vector2>();
        for (int i = 0; i < trailRenderer.positionCount; i++)
        {
            points.Add(trailRenderer.GetPosition(i));
        }

        edgeCollider.SetPoints(points);
    }

    private void OnDestroy()
    {
        Destroy(edgeCollider.gameObject);
    }

}
