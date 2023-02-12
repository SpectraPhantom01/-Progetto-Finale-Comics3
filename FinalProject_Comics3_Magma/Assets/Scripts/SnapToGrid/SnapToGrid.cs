using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private Vector3 gridSize = new Vector3(0.5f,0.5f,0);

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying && this.transform.hasChanged)
            SnapToGrid2();
    }

    private void SnapToGrid2()
    {
        var position = new Vector3(
            Mathf.Round(this.transform.position.x / this.gridSize.x) * this.gridSize.x,
            Mathf.Round(this.transform.position.y / this.gridSize.y) * this.gridSize.y,
            0f);
        this.transform.position = position;
    }
#endif
}
