using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamagedVolumeHandler : MonoBehaviour
{
    [SerializeField] float destinationFadeUp;
    [SerializeField] float destinationFadeDown;
    [SerializeField] float speedFadeUp;
    [SerializeField] float speedFadeDown;
    [SerializeField] float automaticFadeDownTime;

    Volume volume;
    Vignette vignette;
    Coroutine fadingCoroutine;
    private void Awake()
    {
        volume = GetComponentInChildren<Volume>();
        volume.weight = 1;

        volume.profile.TryGet(out vignette);
    }

    public void FadeUp()
    {
        if (fadingCoroutine == null)
            fadingCoroutine = StartCoroutine(FadeCoroutine(speedFadeUp, destinationFadeUp, true));
        else
        {
            StopCoroutine(fadingCoroutine);
            fadingCoroutine = StartCoroutine(FadeCoroutine(speedFadeUp, destinationFadeUp, true));
        }
    }

    public void FadeDown()
    {
        if (fadingCoroutine == null)
            fadingCoroutine = StartCoroutine(FadeCoroutine(speedFadeDown, destinationFadeDown, false));
        else
        {
            StopCoroutine(fadingCoroutine);
            fadingCoroutine = StartCoroutine(FadeCoroutine(speedFadeDown, destinationFadeDown, false));
        }
    }

    private IEnumerator FadeCoroutine(float speed, float destination, bool fadingUp)
    {
        while(vignette.intensity.value != destination)
        {
            yield return new WaitForEndOfFrame();
            if (fadingUp)
            {
                vignette.intensity.value += speed;

                if (vignette.intensity.value >= destination)
                {
                    vignette.intensity.value = destination;
                }
            }
            else
            {
                vignette.intensity.value -= speed;

                if (vignette.intensity.value <= destination)
                {
                    vignette.intensity.value = destination;
                }
            }
        }

        if (fadingUp)
        {
            yield return new WaitForSeconds(automaticFadeDownTime);
            FadeDown();
        }
    }
}
