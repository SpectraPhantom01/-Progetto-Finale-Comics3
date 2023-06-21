using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private AudioListener _audioListner;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        _audioListner = FindObjectOfType<AudioListener>();
    }

    public void ActiveAudio(bool active)
    {
        if(_audioListner == null)
        {
            _audioListner = FindObjectOfType<AudioListener>();
        }

        _audioListner.enabled = active;
        
    }
}
