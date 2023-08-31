using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIInputDeviceSelector : MonoBehaviour, ISubscriber
{
    [SerializeField] EInputDeviceType inputDeviceType;
    TextMeshProUGUI text;
    bool isEnabled = false;
    private void Awake()
    {
        isEnabled = inputDeviceType == EInputDeviceType.MouseAndKeyboard;
        text = gameObject.SearchComponent<TextMeshProUGUI>();

    }

    private void Start()
    {
        SwitchTextOnOff();
        Publisher.Subscribe(this, typeof(InputDeviceChangedMessage));
    }

    private void SwitchTextOnOff()
    {
        switch (inputDeviceType)
        {
            case EInputDeviceType.MouseAndKeyboard:
                isEnabled = LevelManager.Instance.MouseAndKeyboardInputAvailable;
                break;
            case EInputDeviceType.GamePad:
                isEnabled = LevelManager.Instance.GamePadInputAvailable;
                break;
            case EInputDeviceType.JoyStick:
                isEnabled = LevelManager.Instance.JoyStickInputAvailable;
                break;
        }
        text.text = isEnabled ? "ON" : "OFF";
    }

    public void SwitchEnableInputDevice()
    {
        if(LevelManager.Instance.TryEnableInputDevice(!isEnabled, inputDeviceType))
        {
            if (isEnabled)
            {
                isEnabled = false;
                text.text = "OFF";
            }
            else
            {
                isEnabled = true;
                text.text = "ON";
            }
        }
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is InputDeviceChangedMessage)
            text.text = isEnabled ? "ON" : "OFF";
    }

    public void OnDisableSubscribe()
    {
        Publisher.Unsubscribe(this, typeof(InputDeviceChangedMessage));
    }

    private void OnDestroy()
    {
        OnDisableSubscribe();
    }

}
