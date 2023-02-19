using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public PickableScriptableObject PickableScriptableObject;

    private void Start()
    {
        spriteRenderer.sprite = PickableScriptableObject.OnGameSprite;
    }
}
