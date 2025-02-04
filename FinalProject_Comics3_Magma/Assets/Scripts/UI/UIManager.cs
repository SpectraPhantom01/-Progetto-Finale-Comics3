using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public UIPauseMenu PauseMenu;
    public UIPlayArea UIPlayArea;
    [SerializeField] GameObject writtenPanel;
    [SerializeField] TextMeshProUGUI messageText;
    [SerializeField] UIOptions uiOptions;
    [SerializeField] UIContinueButton continueButton;
    [SerializeField] LogScriptableObject logSO;
    [SerializeField] UIDialoguePanel dialoguePanel;
    bool pause = false;
    int indexMessage = 0;
    private List<string> logs;
    private void Awake()
    {
        PauseMenu = pauseMenu.GetComponent<UIPauseMenu>();
    }

    private void Start()
    {
        logs = new();

        if (logSO.Logs == null)
        {
            logSO.Logs = new();
        }
        else
        {
            foreach (var log in logSO.Logs)
            {
                logs.Add(log);
                uiOptions.Add(log);
            }
        }
    }

    public void Pause()
    {
        pause = !pause;

        GameManager.Instance.EnableCursor(pause);

        Time.timeScale = pause ? 0 : 1;

        pauseMenu.SetActive(pause);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenWrittenPanel(string message, List<string> nextMessages = null)
    {
        message = HandleInputControls(message);

        writtenPanel.SetActive(true);
        messageText.text = message;

        Time.timeScale = 0;

        GameManager.Instance.MessageActive = true;

        uiOptions.Add(message);

        logSO.Logs.Add(message);

        if (nextMessages != null && nextMessages.Count > 1)
        {
            List<Coroutine> messagesCoroutine = new();
            indexMessage = nextMessages.Count - 1;
            for (int i = 1; i < nextMessages.Count; i++)
            {
                messagesCoroutine.Add(StartCoroutine(NextMessageCoroutine(i * 0.3f, nextMessages[i])));
            }
        }
    }

    private static string HandleInputControls(string message)
    {
        if (message.Contains("#"))
        {
            var messageSpit = message.Split('#');
            message = string.Empty;
            foreach (var text in messageSpit)
            {
                string castText = text;
                if (text.ToUpper() == "SPACEBAR")
                {
                    if (LevelManager.Instance.JoyStickInputAvailable)
                    {
                        castText = "SQUARE";
                    }
                    else if (LevelManager.Instance.GamePadInputAvailable)
                    {
                        castText = " Y ";
                    }
                }
                else if (text.ToUpper() == "WASD/ARROWS")
                {
                    if (LevelManager.Instance.JoyStickInputAvailable || LevelManager.Instance.GamePadInputAvailable)
                    {
                        castText = "LEFT STICK";
                    }
                }
                else if (text.ToUpper() == "E OR LEFT MOUSE BUTTON")
                {
                    if (LevelManager.Instance.JoyStickInputAvailable)
                    {
                        castText = " X ";
                    }
                    else if (LevelManager.Instance.GamePadInputAvailable)
                    {
                        castText = " B ";
                    }
                }
                else if (text.ToUpper() == "ESC")
                {
                    if (LevelManager.Instance.JoyStickInputAvailable || LevelManager.Instance.GamePadInputAvailable)
                    {
                        castText = "START";
                    }
                }
                else if (text == "1 2 3 4")
                {
                    if (LevelManager.Instance.JoyStickInputAvailable || LevelManager.Instance.GamePadInputAvailable)
                    {
                        castText = "L1 L2 R1 R2";
                    }
                }
                else if (text.ToUpper() == "Q")
                {
                    if (LevelManager.Instance.JoyStickInputAvailable)
                    {
                        castText = "O";
                    }
                    else if (LevelManager.Instance.GamePadInputAvailable)
                    {
                        castText = "A";
                    }
                }

                message += castText;
            }
        }

        return message;
    }

    private IEnumerator NextMessageCoroutine(float time, string message)
    {
        yield return new WaitForSeconds(time);
        OpenWrittenPanel(message);
    }

    public bool CloseWrittenPanelByContinueButton()
    {
        if (!writtenPanel.activeSelf)
            return false;

        if (indexMessage > 0)
        {
            continueButton.Continue();
            indexMessage -= 1;
        }
        else
            continueButton.Continue(pause);

        return true;
    }

    public void NextDialogue()
    {
        dialoguePanel.Next();
    }

    public void OpenGameOverPanel()
    {
        UIPlayArea.OpenGameOverPanel();
    }

    public void ReloadScene()
    {
        Pause();
        GameManager.Instance.Player.PlayerManager.Kill();
    }
}
