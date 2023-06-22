using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PickableObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public PickableScriptableObject PickableScriptableObject;
    public UnityEvent OnPickedUp;
    public bool destroyOnPick = true;
    private void Start()
    {
        spriteRenderer.sprite = PickableScriptableObject.OnGameSprite;
    }

    public void Picked()
    {
        OnPickedUp?.Invoke();
        if(destroyOnPick)
            Destroy(gameObject);
    }
}
