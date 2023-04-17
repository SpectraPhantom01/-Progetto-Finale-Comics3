using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSlime : MonoBehaviour
{
    [SerializeField] GameObject trailPrefab;
    [SerializeField] Transform pivot;
    TrailRenderer trailRenderer;

    public void SpawnTrail()
    {
        if(trailRenderer == null)
        {

            trailRenderer = Instantiate(trailPrefab, pivot).GetComponent<TrailRenderer>();
            trailRenderer.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
    }
    public void BreakTrail()
    {
        if(trailRenderer != null)
        {

            trailRenderer.transform.SetParent(null);
            trailRenderer.autodestruct = true;
            trailRenderer = null;
        }
    }
}
