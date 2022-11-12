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


    internal void SaveInfo(Vector3 transformPosition)
    {
        xPos = transformPosition.x;
        yPos = transformPosition.y;
        zPos = transformPosition.z;
    }
}

[Serializable]
public class SavableInfosList
{
    public List<SavableInfos> SavableInfos;
}
