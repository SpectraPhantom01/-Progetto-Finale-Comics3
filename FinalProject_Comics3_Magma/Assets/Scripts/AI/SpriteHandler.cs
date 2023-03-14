using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    GameObject _graphics;

    private void Awake()
    {
        spriteRenderer = gameObject.SearchComponent<SpriteRenderer>();

        _graphics = spriteRenderer.gameObject;
    }

    private void Update()
    {
        _graphics.transform.LookAt(transform.forward, Vector3.forward);

    }


}
