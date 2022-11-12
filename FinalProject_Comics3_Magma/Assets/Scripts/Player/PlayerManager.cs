using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IAliveEntity
{
    BoxCollider2D _boxCollider;

    public bool IsAlive { get ; set ; }
    public string Name { get ; set ; }

    public void Kill()
    {
        Destroy(gameObject);
    }

    private void Awake()
    {
        _boxCollider = GetComponentInChildren<BoxCollider2D>();
        IsAlive = true;
        Name = "Knight of Time";
    }

}
