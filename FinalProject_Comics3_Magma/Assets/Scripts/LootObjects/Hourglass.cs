using System;
[Serializable]
public class Hourglass : IPickable  
{
    public float Time;
    public Hourglass(float time)
    {
        Time = time;
    }
}
