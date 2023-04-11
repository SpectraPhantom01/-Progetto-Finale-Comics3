using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaSlime : MonoBehaviour
{
    [SerializeField] GameObject trailPrefab;
    TrailRenderer trailRenderer;
    private void Start()
    {
        SpawnTrail();
    }
    public void SpawnTrail()
    {
        trailRenderer = Instantiate(trailPrefab, transform).GetComponent<TrailRenderer>();
    }
    public void BreakTrail()
    {
        if(trailRenderer != null)
        {

            trailRenderer.transform.SetParent(null);
            trailRenderer.autodestruct = true;
        }
    }
}
