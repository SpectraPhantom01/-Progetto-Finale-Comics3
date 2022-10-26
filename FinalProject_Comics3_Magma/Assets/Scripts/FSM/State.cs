using UnityEngine;

public abstract class State
{
    public abstract void OnStart();
    public abstract void OnUpdate();
    public abstract void OnEnd();
    public abstract void OnFixedUpdate();
}
