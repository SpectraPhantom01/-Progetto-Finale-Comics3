using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    bool IsPlayer;

    GameObject _graphics;

    private void Awake()
    {
        spriteRenderer = gameObject.SearchComponent<SpriteRenderer>();
        IsPlayer = gameObject.SearchComponent<PlayerManager>() != null;

        _graphics = spriteRenderer.gameObject;
    }

    private void Update()
    {
        _graphics.transform.localRotation = Quaternion.Euler(90, 0, 0);

        if (IsPlayer)
        {

        }
        else
        {

        }
    }


}
