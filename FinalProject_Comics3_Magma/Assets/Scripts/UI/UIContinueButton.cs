using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContinueButton : MonoBehaviour
{
    public delegate void OnContinueClicked();
    public OnContinueClicked onContinueClicked;

    public void Continue()
    {
        onContinueClicked?.Invoke();
        Time.timeScale = 1.0f;
    }
}
