using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SavableInfos
{
    public float xPos;
    public float yPos;
    public float zPos;
    public float currentHourglassLife;
    public int hourglassQuantity;
    public bool IsPlayer;
    internal void SaveInfo(Vector3 transformPosition, float currentHourglass, int quantityHourglass, bool isPlayer)
    {
        xPos = transformPosition.x;
        yPos = transformPosition.y;
        zPos = transformPosition.z;
        currentHourglassLife = currentHourglass;
        hourglassQuantity = quantityHourglass;
        IsPlayer = isPlayer;
    }
}

[Serializable]
public class SavableInfosList
{
    public List<SavableInfos> SavableInfos;
}
