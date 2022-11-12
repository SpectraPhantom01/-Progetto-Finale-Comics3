using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavableEntity : MonoBehaviour
{


    internal SavableInfos SaveInfo()
    {
        SavableInfos savableInfos = new SavableInfos();
        savableInfos.SaveInfo(transform.position);
        return savableInfos;
    }
}
