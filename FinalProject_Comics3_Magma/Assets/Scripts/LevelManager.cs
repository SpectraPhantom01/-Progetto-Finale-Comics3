using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] GameObject errorPanel;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] Animator animator;
    GameObject loaderCanvas;
    Image progressBar;
    float _target;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        loaderCanvas = GetComponentInChildren<UILoadingBackground>(true).gameObject;
        progressBar = GetComponentInChildren<UIProgressBar>(true).GetComponent<Image>();
    }


    public async void LoadScene(string sceneName)
    {
        _target = 0;
        progressBar.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loaderCanvas.SetActive(true);

        do
        {
            await Task.Delay(100);
            _target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);

        scene.allowSceneActivation = true;

        await Task.Delay(2000);

        animator.SetTrigger("Close");
    }

    public async Task LoadSceneAsync(string sceneName)
    {
        _target = 0;
        progressBar.fillAmount = 0;

        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loaderCanvas.SetActive(true);

        do
        {
            await Task.Delay(100);
            _target = scene.progress;
        } while (scene.progress < 0.9f);

        await Task.Delay(1000);

        scene.allowSceneActivation = true;

        await Task.Delay(2000);

        animator.SetTrigger("Close");
    }

    public void CloseCanvas()
    {

        loaderCanvas.SetActive(false);
    }

    private void Update()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, _target, 3 * Time.deltaTime);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    //void OnEnable()
    //{
    //    Application.logMessageReceived += LogCallback;
    //}

    ////Called when there is an exception
    //public void LogCallback(string condition, string stackTrace, LogType type)
    //{
    //    if (type == LogType.Error || type == LogType.Exception)
    //    {
    //        errorPanel.SetActive(true);
    //        errorText.text = condition + " - " + stackTrace;
    //    }
    //}

    //void OnDisable()
    //{
    //    Application.logMessageReceived -= LogCallback;
    //}

    internal void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }
}
