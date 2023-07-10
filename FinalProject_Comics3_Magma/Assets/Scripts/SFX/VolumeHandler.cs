using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeHandler : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float fadeSpeed;
    [SerializeField] float destinationVolume;

    public void Fade()
    {
        if (!gameObject.activeSelf) return;

        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        while(audioSource.volume != destinationVolume)
        {
            audioSource.volume = Mathf.Lerp(audioSource.volume, destinationVolume, fadeSpeed);
            yield return null;
            if(audioSource.volume - destinationVolume <= fadeSpeed)
                audioSource.volume = destinationVolume;
        }

        if(destinationVolume == 0)
        {
            gameObject.SetActive(false);
        }
    }
}
