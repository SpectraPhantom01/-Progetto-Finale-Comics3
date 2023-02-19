using System;
[Serializable]
public class Hourglass : IPickable  
{
    public float Time;
    public float HourglassLife = 100;
    public float BaseTimeLoseSand;
    public float RealTimeLoseSand => HourglassLife == 100 ? BaseTimeLoseSand : BaseTimeLoseSand - (MathF.Abs(HourglassLife -100) * (BaseTimeLoseSand - MaxSpeedLoseSand) / 100);
    public float MaxSpeedLoseSand;

#if UNITY_EDITOR
    public float DEBUGLossXsec = 1;
#endif
    public Hourglass(float time)
    {
        Time = time;
        HourglassLife = 100;
    }

    public void Damage(float percentage)
    {
        HourglassLife -= percentage;
        if(HourglassLife < 0)
            HourglassLife = 0;
    }

    public void Heal(float percentage)
    {
        HourglassLife += percentage;
        if (HourglassLife > 100)
            HourglassLife = 100;
    }
#if UNITY_EDITOR
    public void Calculate_TimeLossXsecond()
    {
        DEBUGLossXsec = 1/RealTimeLoseSand;
    }
#endif
}
