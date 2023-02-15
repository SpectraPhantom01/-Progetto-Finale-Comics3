using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Damager : MonoBehaviour
{

    [SerializeField] Transform damagerArea;
    [SerializeField] LayerMask _damageableMask;
    [SerializeField] Vector2 hitBox = Vector2.one;
    [Tooltip("Only for TRIGGER Damagers such as Slime Trail")]
    [SerializeField] float triggerDamage;
    bool _isPlayer;
    List<AttackScriptableObject> _attackList;
    PlayerManager _playerManager;
    AI _ai;
    AttackScriptableObject _equippedAttack;

    public AttackScriptableObject EquippedAttack => _equippedAttack;
    private void Awake()
    {
        _playerManager = gameObject.SearchComponent<PlayerManager>();

        _isPlayer = _playerManager != null;

        if (!_isPlayer)
        {
            _ai = gameObject.SearchComponent<AI>();
            //if (_ai == null)
            //    throw new Exception("ERRORE: non è stato possibile trovare il componente Player Manager o AI");
        }

        if(_isPlayer || _ai != null)
            _attackList = gameObject.SearchComponent<IAliveEntity>().AttackList;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var attack = _attackList?.Find(x => x.AttackType == EAttackType.Contact);
        if (attack != null)
        {
            if (!_isPlayer && _damageableMask.Contains(collision.gameObject.layer))
            {
                Damageable damageable = collision.gameObject.SearchComponent<Damageable>();
                if (damageable != null)
                    GiveDamage(damageable, attack, transform);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_isPlayer && _damageableMask.Contains(collision.gameObject.layer))
        {
            Damageable damageable = collision.gameObject.SearchComponent<Damageable>();
            if (damageable != null)
                damageable.Damage(triggerDamage, 20, -(transform.position - damageable.transform.position).normalized);
        }
    }

    private void AttackMelee()
    {
        if(_equippedAttack != null)
        {
            var collidersHit = Physics2D.OverlapBoxAll(damagerArea.position, hitBox, 0, _damageableMask).ToList();
            if (collidersHit.Count > 0)
            {
                var damageableList = collidersHit
                    .Where(x => _damageableMask.Contains(x.gameObject.layer))
                    .Select(x => x.gameObject.SearchComponent<Damageable>())
                    .Where(x => x != null).ToList();

                damageableList.ForEach(damageable => GiveDamage(damageable, _equippedAttack, transform));
            }

        }
        
    }

    public void SizeUpHitBox()
    {
        hitBox = _equippedAttack.hitBoxRangeDamage;
    }
    
    public void SizeDownHitBox()
    {
        hitBox = _equippedAttack.hitBoxRangeView;
    }

    private void AttackShoot()
    {
        if (_equippedAttack != null)
        {
            var collidersHit = Physics2D.OverlapCircleAll(transform.position, _equippedAttack.shootAttackRangeOfView, _damageableMask).ToList();
            if (collidersHit.Count > 0)
            {
                var damageableList = collidersHit
                    .Where(x => _damageableMask.Contains(x.gameObject.layer))
                    .Select(x => x.gameObject.SearchComponent<Damageable>())
                    .Where(x => x != null).ToList();

                if (damageableList.Count > 0)
                {
                    SpellBullet spellBullet = Instantiate(_equippedAttack.SpellPrefab, damagerArea.position, Quaternion.identity);
                    spellBullet.Initialize(_damageableMask, _equippedAttack, _isPlayer ? _playerManager.CurrentDirection : _ai.CurrentDirection, gameObject.layer, (damageableList.Min(x => x.transform.position) - transform.position).normalized);
                }
            }
        }
    }

    public void Attack()
    {
        switch (_equippedAttack.AttackType)
        {
            case EAttackType.Melee:
                AttackMelee();
                break;
            case EAttackType.Shoot:
                AttackShoot();
                break;
        }
    }

    public void EquipAttack(EAttackType eAttackType)
    {
        switch (eAttackType)
        {
            case EAttackType.Melee:
                _equippedAttack = _attackList.Find(x => x.AttackType == EAttackType.Melee);
                break;
            case EAttackType.Shoot:
                _equippedAttack = _attackList.Find(x => x.AttackType == EAttackType.Shoot);
                break;
        }

        SizeDownHitBox();
    }

    public void EquipAttackMeleeSubCategory(EMeleeAttackType eMeleeAttackType)
    {
        switch (eMeleeAttackType)
        {
            case EMeleeAttackType.Sword:
                _equippedAttack = _attackList.Find(x => x.MeleeAttackType == EMeleeAttackType.Sword);
                break;
            case EMeleeAttackType.Punch:
                _equippedAttack = _attackList.Find(x => x.MeleeAttackType == EMeleeAttackType.Punch);
                break;
        }
    }

    public void EquipAttackShootSubCategory(EShootingAttackType eShootingAttackType)
    {
        switch (eShootingAttackType)
        {
            case EShootingAttackType.Spell:
                _equippedAttack = _attackList.Find(x => x.ShootingAttackType == EShootingAttackType.Spell);
                break;
            case EShootingAttackType.Arrow:
                _equippedAttack = _attackList.Find(x => x.ShootingAttackType == EShootingAttackType.Arrow);
                break;
        }
    }

    public static void GiveDamage(Damageable damageable, AttackScriptableObject attack, Transform transform)
    {
        damageable.Damage(attack.DamageAmount, attack.KnockBack, -(transform.position - damageable.transform.position).normalized);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (damagerArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(damagerArea.position, hitBox);
        }
    }
#endif
}
