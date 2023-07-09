using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIContinueButton : MonoBehaviour
{
    public delegate void OnContinueClicked();
    public OnContinueClicked onContinueClicked;
    public GameObject writtenPanel;
    public void Continue(bool gamePaused = false)
    {
        onContinueClicked?.Invoke();
        Time.timeScale = gamePaused ? 0 : 1.0f;
        writtenPanel.SetActive(false);
    }
}
