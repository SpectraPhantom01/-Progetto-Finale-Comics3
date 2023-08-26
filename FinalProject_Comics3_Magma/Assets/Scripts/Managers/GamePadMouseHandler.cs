using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    internal void Click()
    {
        throw new NotImplementedException();
    }
}
