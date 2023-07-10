using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffectGenerator : MonoBehaviour
{
    [SerializeField] CinemachineImpulseSource impulse;
    [SerializeField] ParticleSystem cedibleRoofParticle;
    [SerializeField] AudioSource sfx;
    [SerializeField] float frequency;

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
            sfx.Play();
            yield return new WaitForSeconds(frequency);
        }
    }

}
