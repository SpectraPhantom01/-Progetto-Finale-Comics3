using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SoundManager>();
                if (instance != null)
                    return instance;

                GameObject go = new("SoundManager");
                return go.AddComponent<SoundManager>();
            }
            else
                return instance;
        }
        set
        {
            instance = value;
        }
    }

    public AudioMixer AudioMixer;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }
    public void ActiveAudio(bool active)
    {
        if (active)
        {

            AudioMixer.SetFloat("Master", 0);
        }
        else
        {

            AudioMixer.SetFloat("Master", -80);
        }
    }
}
