using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecroHybrid : MonoBehaviour
{
    [SerializeField] LayerMask invulnerableLayer;
    [SerializeField] LayerMask vulnerableLayer;
    [SerializeField] GameObject physicsObject;
    private void Awake()
    {
        gameObject.layer = invulnerableLayer.ToLayer();
    }

    public void SetVulnerable(bool vulnerable)
    {
        physicsObject.layer = vulnerable ? vulnerableLayer.ToLayer() : invulnerableLayer.ToLayer();

    }
}

