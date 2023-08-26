using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] GameObject errorPanel;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] Animator animator;
    GameObject loaderCanvas;
    Image progressBar;
    float _target;

    public bool JoyStickInputAvailable { get; private set; }
    public bool GamePadInputAvailable { get; private set; }
    public bool MouseAndKeyboardInputAvailable { get; private set; } = true;
    private void Awake()
    {
        if (Instance == null)
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

        TryEnableInputDevice(true, EInputDeviceType.GamePad);
        TryEnableInputDevice(true, EInputDeviceType.JoyStick);
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
    public static bool FindInputDevice(string device)
    {
        var joysticks = Input.GetJoystickNames().ToList();
        return joysticks.Any(j => j.ToLower().Contains(device));
    }
    public bool TryEnableInputDevice(bool enable, EInputDeviceType inputDeviceType)
    {
        switch (inputDeviceType)
        {
            case EInputDeviceType.MouseAndKeyboard:

                if (!enable && (JoyStickInputAvailable || GamePadInputAvailable))
                    MouseAndKeyboardInputAvailable = false;
                else
                {
                    MouseAndKeyboardInputAvailable = true;
                    Publisher.Publish(new InputDeviceChangedMessage());
                    return true;
                }

                return false;
            case EInputDeviceType.GamePad:
                if (FindInputDevice("gamepad"))
                    GamePadInputAvailable = enable;
                else
                    return false;
                break;
            case EInputDeviceType.JoyStick:
                if (FindInputDevice("joystick"))
                    JoyStickInputAvailable = enable;
                else
                    return false;
                break;
        }

        Publisher.Publish(new InputDeviceChangedMessage());
        return true;
    }
}

public enum EInputDeviceType
{
    MouseAndKeyboard,
    GamePad,
    JoyStick
}