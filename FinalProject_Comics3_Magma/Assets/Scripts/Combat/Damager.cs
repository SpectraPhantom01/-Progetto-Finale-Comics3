using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Damager : MonoBehaviour
{

    [SerializeField] Transform damagerArea;
    [SerializeField] LayerMask _damageableMask;
    [SerializeField] LayerMask _interactableMask; //Prova
    [SerializeField] Vector2 hitBox = Vector2.one;
    [Tooltip("Only for TRIGGER Damagers such as Slime Trail")]
    [SerializeField] float triggerDamage;
    [SerializeField] float contactKnockback = 20;
    [Tooltip("QUESTO DEVE ESSERE SEMPRE ESPRESSO IN PERCENTUALE")]
    [SerializeField] float triggerHourglassPercentageDamage;
    [SerializeField] bool disableTriggerAfterFirstEnter;
    bool _isPlayer;
    List<AttackScriptableObject> _attackList;
    PlayerManager _playerManager;
    AI _ai;
    AttackScriptableObject _equippedAttack;
    float _bonusAttackPercentage;
    public AttackScriptableObject EquippedAttack => _equippedAttack;
    private void Awake()
    {
        _playerManager = gameObject.SearchComponent<PlayerManager>();

        _isPlayer = _playerManager != null;

        if (!_isPlayer)
        {
            _ai = gameObject.SearchComponent<AI>();
            //if (_ai == null)
            //    throw new Exception("ERRORE: non � stato possibile trovare il componente Player Manager o AI");
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
                    GiveDamage(damageable, attack, transform, _bonusAttackPercentage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //var player = collision.gameObject.SearchComponent<PlayerManager>();
        if (!_isPlayer && _damageableMask.Contains(collision.gameObject.layer) /*&& !player.PlayerController.IsDashing*/)
        {
            Damageable damageable = collision.gameObject.SearchComponent<Damageable>();
            if (damageable != null)
                damageable.Damage(triggerDamage, contactKnockback, -(transform.position - damageable.transform.position).normalized, triggerHourglassPercentageDamage);

            if (disableTriggerAfterFirstEnter)
                gameObject.SearchComponent<Collider2D>().enabled = false;
        }
    }

    private bool AttackMelee()
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

                damageableList.ForEach(damageable => GiveDamage(damageable, _equippedAttack, transform, _bonusAttackPercentage));

                return damageableList.Count > 0;
            }

        }
        return false;
    }

    public bool SearchInteractable()
    {
        var collidersHit = Physics2D.OverlapBoxAll(damagerArea.position, hitBox, 0, _interactableMask).ToList();
        if(collidersHit.Count > 0)
        {
            return SearchPillars(collidersHit);
        }
        return false;
    }

    private bool SearchPillars(List<Collider2D> collidersHit)
    {
        var interactableList = collidersHit
                          .Where(x => _interactableMask.Contains(x.gameObject.layer))
                          .Select(x => x.gameObject.SearchComponent<Pillar>())
                          .Where(x => x != null).ToList();
        interactableList.ForEach(interactable => interactable.PillarDestruction());
        return interactableList.Count > 0;
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
                    spellBullet.Initialize(_damageableMask, _equippedAttack, gameObject.layer, (damageableList.Min(x => x.transform.position) - damagerArea.position).normalized);
                }
            }
        }
    }

    public bool Attack()
    {
        switch (_equippedAttack.AttackType)
        {
            case EAttackType.Melee:
                return AttackMelee();
                
            case EAttackType.Shoot:
                AttackShoot();
                break;
        }
        return false;
    }

    public void EquipAttack(EAttackType eAttackType)
    {

        _equippedAttack = _attackList.Find(x => x.AttackType == eAttackType);

        SizeDownHitBox();
    }

    public void EquipAttackMeleeSubCategory(EMeleeAttackType eMeleeAttackType)
    {
        _equippedAttack = _attackList.Find(x => x.MeleeAttackType == eMeleeAttackType);
    }

    public void EquipAttackShootSubCategory(EShootingAttackType eShootingAttackType)
    {
        _equippedAttack = _attackList.Find(x => x.ShootingAttackType == eShootingAttackType);
    }

    public void SetBonusAttack(float bonus1, float bonus2)
    {
        _bonusAttackPercentage = bonus1 + bonus2;
    }

    public static void GiveDamage(Damageable damageable, AttackScriptableObject attack, Transform transform, float bonus)
    {
        var damage = attack.DamageAmount + (attack.DamageAmount * bonus / 100);
        damageable.Damage(damage, attack.KnockBack, -(transform.position - damageable.transform.position).normalized, attack.HourglassPercentageDamageAmount);
    }

    public void Initialize(List<AttackScriptableObject> attackList, Transform damagerArea)
    {
        
        this.damagerArea = damagerArea;
        _attackList = attackList;
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
