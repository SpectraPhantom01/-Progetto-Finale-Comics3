using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] GameObject errorPanel;
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] Animator animator;
    [SerializeField] List<PickableScriptableObject> pickableSOInstances;
    GameObject loaderCanvas;
    Image progressBar;
    float _target;
    private Action onContinueButton;
    public bool JoyStickInputAvailable { get; private set; }
    public bool GamePadInputAvailable { get; private set; }
    public bool MouseAndKeyboardInputAvailable { get; private set; } = true;
    Coroutine deviceCheck;
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

    private void Start()
    {
        DeviceCheck();
    }

    private void DeviceCheck()
    {
        if(deviceCheck == null)
            deviceCheck = StartCoroutine(DevicesCheckCoroutine());
        else
        {
            StopCoroutine(deviceCheck);
            deviceCheck = StartCoroutine(DevicesCheckCoroutine());
        }
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

    private IEnumerator DevicesCheckCoroutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(2);
            if (JoyStickInputAvailable)
            {
                JoyStickInputAvailable = FindInputDevice("joystick");
                if (!JoyStickInputAvailable)
                {
                    SendNotDeviceDetectedMessage("USB Joystick");
                }
            }

            if (GamePadInputAvailable)
            {
                GamePadInputAvailable = FindInputDevice("gamepad");
                if (!GamePadInputAvailable)
                {
                    SendNotDeviceDetectedMessage("GamePad");

                }
            }

            if (!JoyStickInputAvailable && !GamePadInputAvailable)
                break;
        }

        
    }

    private void SendNotDeviceDetectedMessage(string deviceName)
    {
        errorPanel.SetActive(true);
        errorText.text = $"WARNING: {deviceName} Disconnected! Connect the device again and press the continue button.";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DeviceCheck();

        onContinueButton += CheckAgaintDevices;
    }

    private void CheckAgaintDevices()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        JoyStickInputAvailable = FindInputDevice("joystick");
        GamePadInputAvailable = FindInputDevice("gamepad");
        if (JoyStickInputAvailable || GamePadInputAvailable)
        {
            DeviceCheck();
        }
        else
        {
            errorPanel.SetActive(true);
            errorText.text = $"WARNING: NO DEVICE FOUND. Try restart the Game or play with Mouse and Keyboard";
        }
        onContinueButton -= CheckAgaintDevices;
        onContinueButton += DisableCursorContinueButton;
        Mouse.current.WarpCursorPosition(Input.mousePosition);
        Publisher.Publish(new InputDeviceChangedMessage());
    }

    private void DisableCursorContinueButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        onContinueButton -= DisableCursorContinueButton;
    }

    public void ContinueAction()
    {
        onContinueButton?.Invoke();
    }

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

        DeviceCheck();
        Publisher.Publish(new InputDeviceChangedMessage());
        return true;
    }

    public PickableScriptableObject GetPickable(EPickableEffectType effectType)
    {
        return pickableSOInstances.FirstOrDefault(x => x.PickableEffectType == effectType);
    }
}
