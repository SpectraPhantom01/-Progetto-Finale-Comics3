using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISoundHandler : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] string textSoundOn;
    [SerializeField] string textSoundOff;

    bool soundOn = true;

    public void SwitchSound()
    {
        if (soundOn)
        {
            soundOn = false;
            text.text = textSoundOff;
        }
        else
        {
            soundOn = true;
            text.text = textSoundOn;
        }

        SoundManager.Instance.ActiveAudio(soundOn);
    }

}
