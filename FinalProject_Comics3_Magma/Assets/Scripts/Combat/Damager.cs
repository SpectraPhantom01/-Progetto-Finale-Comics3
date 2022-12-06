using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] Transform damagerArea;
    [SerializeField] float _damageAmount;
    [SerializeField] LayerMask _damageableMask;
    [SerializeField] float radius;
    [SerializeField] float knockBack;
    [SerializeField] Vector2 hitBox = Vector2.one;
    bool _isPlayer;

    private void Awake()
    {
        _isPlayer = gameObject.SearchComponent<PlayerManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isPlayer && _damageableMask.Contains(collision.gameObject.layer))
        {
            Damageable damageable = collision.gameObject.SearchComponent<Damageable>();
            GiveDamage(damageable);
        }
    }

    private void GiveDamage(Damageable damageable)
    {
        damageable.Damage(_damageAmount, knockBack, -(transform.position - damageable.transform.position).normalized);
    }

    public void Attack()
    {
        var collidersHit = Physics2D.OverlapBoxAll(damagerArea.position, hitBox, 0, _damageableMask).ToList();
        if (collidersHit.Count > 0)
        {
            var damageableList = collidersHit
                .Where(x => _damageableMask.Contains(x.gameObject.layer))
                .Select(x => x.gameObject.SearchComponent<Damageable>())
                .Where(x => x != null).ToList();

            damageableList.ForEach(damageable => GiveDamage(damageable));
        }

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(damagerArea != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(damagerArea.position, hitBox);
        }
    }
#endif
}
