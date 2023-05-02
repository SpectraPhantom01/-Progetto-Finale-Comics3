using UnityEngine;

public class Luminosita : MonoBehaviour
{
    public float speed = 1.0f;          // velocità di transizione impostabile dall'editor
    public float minAlpha = 0.0f;       // alfa minima impostabile dall'editor
    public float maxAlpha = 1.0f;       // alfa massima impostabile dall'editor
    public Color baseColor;             // colore di sfondo
    public Color color1;                // primo colore di transizione impostabile dall'editor
    public Color color2;                // secondo colore di transizione impostabile dall'editor
    public Color color3;                // terzo colore di transizione impostabile dall'editor

    private Color targetColor;          // colore di transizione corrente
    private float alpha;                // alfa corrente
    private bool isIncreasing = true;   // indica se stiamo aumentando o diminuendo l'alfa

    void Start()
    {
        // inizializziamo il colore di transizione corrente
        targetColor = color1;
    }

    void Update()
    {
        // calcoliamo il nuovo valore di alfa
        if (isIncreasing)
        {
            alpha += Time.deltaTime * speed;
            if (alpha >= maxAlpha)
            {
                alpha = maxAlpha;
                isIncreasing = false;
            }
        }
        else
        {
            alpha -= Time.deltaTime * speed;
            if (alpha <= minAlpha)
            {
                alpha = minAlpha;
                isIncreasing = true;

                // cambiamo colore di transizione
                if (targetColor == color1)
                {
                    targetColor = color2;
                }
                else if (targetColor == color2)
                {
                    targetColor = color3;
                }
                else
                {
                    targetColor = color1;
                }
            }
        }

        // aggiorniamo il colore di sfondo del GameObject
        Color newColor = Color.Lerp(baseColor, targetColor, alpha);
        GetComponent<Renderer>().material.color = newColor;
    }
}
