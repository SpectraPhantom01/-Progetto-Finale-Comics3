using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombVFX : MonoBehaviour
{
    public delegate void OnExplosion();
    public OnExplosion onExplosion;
    public void Explode()
    {
        onExplosion?.Invoke();
    }
}
