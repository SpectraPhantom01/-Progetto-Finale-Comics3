using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region SINGLETON
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance != null)
                    return instance;

                GameObject go = new GameObject("UIManager");
                return go.AddComponent<UIManager>();
            }
            else
                return instance;
        }
        set
        {
            instance = value;
        }
    }

    #endregion

    [SerializeField] GameObject pauseMenu;
    public UIPlayArea UIPlayArea;
    bool pause = false;
    public void Pause()
    {
        pause = !pause;

        Time.timeScale = pause ? 0 : 1;

        pauseMenu.SetActive(pause);
    }
}
