using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewAttackType", menuName ="ScriptableObject/AttackType")]
public class AttackScriptableObject : ScriptableObject
{
    public EAttackType AttackType;
    public float DamageAmount;
    public float HourglassPercentageDamageAmount;
    public float KnockBack;
    public Vector2 hitBoxRangeView;
    public Vector2 hitBoxRangeDamage;
    [Space(20)]
    [Header("MELEE ONLY!!!")]
    public EMeleeAttackType MeleeAttackType = EMeleeAttackType.none;
    [Space(20)]
    [Header("SHOOTING ONLY!!!")]
    public EShootingAttackType ShootingAttackType = EShootingAttackType.none;
    public SpellBullet SpellPrefab;
    public float shootAttackRangeOfView;
    public float bulletFixedSpeed;
    public float bulletLifeTime;
}

public enum EMeleeAttackType
{
    none,
    Sword,
    Punch
}

public enum EShootingAttackType
{
    none,
    Spell,
    Arrow
}
