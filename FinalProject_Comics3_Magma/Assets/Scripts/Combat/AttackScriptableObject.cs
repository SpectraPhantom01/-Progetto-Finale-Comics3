using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewAttackType", menuName ="ScriptableObject/AttackType")]
public class AttackScriptableObject : ScriptableObject
{
    public EAttackType AttackType;
    public float DamageAmount;
    public float KnockBack;
    [Space(20)]
    [Header("BULLETS ONLY!!!")]
    public SpellBullet SpellPrefab;
    public float shootAttackRangeOfView;
    public float bulletFixedSpeed;
    public float bulletLifeTime;
}
