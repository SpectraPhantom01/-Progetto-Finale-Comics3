using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Autodestroy : MonoBehaviour
{
    public float time;
    public bool DestroyOnEnd;
    public bool IgnoreCountdown = false;
    public UnityEvent Event;
    private void OnEnable()
    {
        if(!IgnoreCountdown)
            Invoke(nameof(CallEvent), time);
        if (DestroyOnEnd)
            Destroy(gameObject, time + 0.5f);
    }

    public void CallEvent()
    {
        Event.Invoke();
    }
}
