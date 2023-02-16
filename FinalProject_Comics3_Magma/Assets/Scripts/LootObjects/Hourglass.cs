using System;
[Serializable]
public class Hourglass : IPickable  
{
    public float Time;
    public float HourglassLife;
    public Hourglass(float time)
    {
        Time = time;
        HourglassLife = 100;
    }

    public bool Damage(float percentage)
    {
        HourglassLife -= percentage;
        if(HourglassLife < 0)
            Time = 0;

        return Time > 0;
    }

    public void Heal(float percentage)
    {
        HourglassLife += percentage;
        if (HourglassLife > 100)
            HourglassLife = 100;
    }
}
