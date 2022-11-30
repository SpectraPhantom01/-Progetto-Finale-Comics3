using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavableEntity : MonoBehaviour
{
    internal SavableInfos SaveInfo()
    {
        SavableInfos savableInfos = new();
        Damageable damageable = gameObject.SearchComponent<Damageable>();
        savableInfos.SaveInfo(transform.position, damageable.CurrentTimeLife, damageable.Hourglasses, gameObject.SearchComponent<PlayerManager>() != null);
        return savableInfos;
    }
}
