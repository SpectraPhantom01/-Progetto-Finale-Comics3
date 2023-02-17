using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Pickable", menuName = "ScriptableObject/New Pickable")]
public class PickableScriptableObject : ScriptableObject
{
    [Header("Meta Settings")]
    public string ObjectName;
    [TextArea] public string ObjectDescription;
    public Sprite ObjectInventorySprite;
    public Sprite OnGameSprite;

    [Header("Effect Settings")]
    public bool IsConsumable;
    public bool IsKeyObject;
    public int QuantityOnPick = 1;
    public EPickableEffectType PickableEffectType;
    public float EffectInPercentage;
}

public enum EPickableEffectType
{
    AddAttackForce,
    HealTime,
    HealHourglass
}
