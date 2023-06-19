using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DancingLight : MonoBehaviour
{
    [SerializeField] Light2D _light;
    [SerializeField] float minFallOfIntensity;
    [SerializeField] float maxFallOfIntensity;
    [SerializeField] float timeSpeed;
    [SerializeField] float maxTime;
    float timePassed;

    private void Start()
    {
        _light.falloffIntensity = minFallOfIntensity;
        timePassed = 0;
    }

    private void Update()
    {
        timePassed += Time.deltaTime * timeSpeed;
        if(timePassed >= maxTime)
        {
            _light.intensity = Random.Range(minFallOfIntensity, maxFallOfIntensity);
            timePassed = 0;
        }

    }
}
