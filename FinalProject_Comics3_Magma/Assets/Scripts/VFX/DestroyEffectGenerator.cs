using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffectGenerator : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource impulse;
    [SerializeField] ParticleSystem cedibleRoofParticle;
    [SerializeField] ParticleSystem smokeParticle;
    [SerializeField] AudioSource sfx;
    [SerializeField] Vector2 randomFrequency;

    private void Start()
    {
        StartCoroutine(ImpulseCoroutine());
    }

    public IEnumerator ImpulseCoroutine()
    {
        while (true)
        {
            impulse.GenerateImpulse();
            cedibleRoofParticle.Play();
            smokeParticle.Play();
            sfx.Play();
            yield return new WaitForSeconds(Random.Range(randomFrequency.x, randomFrequency.y));
        }
    }

}
