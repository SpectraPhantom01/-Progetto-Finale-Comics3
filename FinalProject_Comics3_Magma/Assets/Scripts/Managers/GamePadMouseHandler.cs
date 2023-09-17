using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamePadMouseHandler : MonoBehaviour
{
    Vector2 direction;
    public bool Active;
    public void SetDirection(Vector2 direc) { direction = direc; }
    
    private void Update()
    {
        if(Active)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Mouse.current.WarpCursorPosition(direction * 10 + mousePosition);
        }
    }

    public void Click()
    {
        var hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = Input.mousePosition }, hits);
        if(hits.Count > 0)
        {
            foreach(var hit in hits)
            {
                if(hit.gameObject.transform.TryGetComponent<Button>(out var button))
                {
                    if(button.interactable)
                        button.onClick.Invoke();
                    return;
                }
            }
        }
    }
}
