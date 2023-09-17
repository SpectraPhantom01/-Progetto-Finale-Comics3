using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Pickable", menuName = "ScriptableObject/New Pickable")]
[Serializable]
public class PickableScriptableObject : ScriptableObject
{
    [Header("Meta Settings")]
    public string ObjectName;
    [TextArea] public string ObjectDescription;

    [Header("Sprite Settings")]
    public Sprite ObjectInventorySprite;
    public Sprite OnGameSprite;

    [Header("Effect Settings")]
    public bool IsConsumable;
    public bool IsKeyObject;
    public int QuantityOnPick = 1;
    public EPickableEffectType PickableEffectType;
    [Tooltip("Only effects in Percentage")]
    [Range(0, 100)] public float EffectInPercentage;
    [Tooltip("Only effects in Time")]
    public float EffectInTime;

    [Header("Spawn Bomb")]
    public Bomb Bomb;
    public static void UseActiveObject(Pickable objectToUse, Damageable damageable, Transform attackPosition, PlayerManager playerManager)
    {
        switch (objectToUse.PickableSO.PickableEffectType)
        {
            case EPickableEffectType.HealHourglass:
                damageable.HealHourglass(objectToUse.PickableSO.EffectInPercentage);
                break;
            case EPickableEffectType.ThrowBomb:
                var newBomb = Instantiate(objectToUse.PickableSO.Bomb, attackPosition.position + (Vector3.up / 2), Quaternion.identity);
                newBomb.Initialize(playerManager.CurrentVectorDirection);
                break;
            case EPickableEffectType.StopHourglass:
                playerManager.StopHourglass(objectToUse.PickableSO.EffectInTime);
                break;
            case EPickableEffectType.ReduceDamageAndKnockback:
                damageable.StartNewResistance(objectToUse.PickableSO.EffectInPercentage, objectToUse.PickableSO.EffectInTime);
                break;
            case EPickableEffectType.AddHourglass:
                damageable.Hourglasses.Add(new Hourglass(500));
                playerManager.AddNewHourglass();
                break;
        }
    }

    public bool IsEqual(PickableScriptableObject pickable)
    {
        return ObjectName == pickable.ObjectName
            && ObjectDescription == pickable.ObjectDescription;
    }

    public bool IsActiveObject() => IsConsumable;
}

public enum EPickableEffectType
{
    none = 0,
    AddAttackForce = 1,
    AddGhostTime = 2,
    AddMovementSpeed = 3,
    HealHourglass = 4,
    ThrowBomb = 5,
    StopHourglass = 6,
    ReduceDamageAndKnockback = 7,
    AddHourglass = 8,
    Key = 9
}
