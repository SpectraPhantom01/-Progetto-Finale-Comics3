using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeColor : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float fadeSpeed;
    [SerializeField] float destinationAlpha;
    bool sprenderer;
    private void Awake()
    {
        sprenderer = spriteRenderer != null;
    }
    public void Fade()
    {
        if (!gameObject.activeSelf) return;

        if(sprenderer)
            StartCoroutine(FadeSpriteRendererCoroutine());
        else
            StartCoroutine(FadeImageCoroutine());
    }

    private IEnumerator FadeImageCoroutine()
    {
        while (image.color.a != destinationAlpha)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(image.color.a, destinationAlpha, fadeSpeed));
            yield return new WaitForEndOfFrame();
            if (Mathf.Abs(image.color.a - destinationAlpha) <= fadeSpeed)
                image.color = new Color(image.color.r, image.color.g, image.color.b, destinationAlpha);
        }

    }


    private IEnumerator FadeSpriteRendererCoroutine()
    {
        while (spriteRenderer.color.a != destinationAlpha)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, Mathf.Lerp(spriteRenderer.color.a, destinationAlpha, fadeSpeed));
            yield return new WaitForEndOfFrame();
            if (Mathf.Abs(spriteRenderer.color.a - destinationAlpha) <= fadeSpeed)
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, destinationAlpha);
        }

    }
}
