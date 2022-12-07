using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Damager : MonoBehaviour
{

    [SerializeField] Transform damagerArea;
    [SerializeField] LayerMask _damageableMask;
    [SerializeField] Vector2 hitBox = Vector2.one;
    bool _isPlayer;
    List<AttackScriptableObject> _attackList;
    PlayerManager _playerManager;
    AI _ai;

    private void Awake()
    {
        _playerManager = gameObject.SearchComponent<PlayerManager>();

        _isPlayer = _playerManager != null;

        if (!_isPlayer)
        {
            _ai = gameObject.SearchComponent<AI>();
            if (_ai == null)
                throw new Exception("ERRORE: non è stato possibile trovare il componente Player Manager o AI");
        }

        _attackList = gameObject.SearchComponent<IAliveEntity>().AttackList;
        for (int i = 0; i < _attackList.Count; i++)
        {
            for (int j = i + 1; j < _attackList.Count - 1; j++)
            {
                if (_attackList[i].AttackType == _attackList[j].AttackType)
                    throw new Exception($"ERRORE: nel gameObject {gameObject.name} ci sono attacchi doppi: {_attackList[i].AttackType} negli indici {i} e {j}. Questo non può accadere, Damager annullato!!!");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var attack = _attackList.Find(x => x.AttackType == EAttackType.Contact);
        if(attack != null)
        {
            if (!_isPlayer && _damageableMask.Contains(collision.gameObject.layer))
            {
                Damageable damageable = collision.gameObject.SearchComponent<Damageable>();
                if (damageable != null)
                    GiveDamage(damageable, attack, transform);
            }
        }
    }

    public void AttackMelee()
    {
        var attack = _attackList.Find(x => x.AttackType == EAttackType.Melee);
        if(attack != null)
        {
            var collidersHit = Physics2D.OverlapBoxAll(damagerArea.position, hitBox, 0, _damageableMask).ToList();
            if (collidersHit.Count > 0)
            {
                var damageableList = collidersHit
                    .Where(x => _damageableMask.Contains(x.gameObject.layer))
                    .Select(x => x.gameObject.SearchComponent<Damageable>())
                    .Where(x => x != null).ToList();

                damageableList.ForEach(damageable => GiveDamage(damageable, attack, transform));
            }
        }
        
    }

    public void AttackShoot()
    {
        var attack = _attackList.Find(x => x.AttackType == EAttackType.Shoot);
        if (attack != null)
        {
            SpellBullet spellBullet = Instantiate(attack.SpellPrefab, damagerArea.position, Quaternion.identity);
            spellBullet.Initialize(_damageableMask, attack, _isPlayer ? _playerManager.CurrentDirection : _ai.CurrentDirection, gameObject.layer);
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
