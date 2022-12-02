using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damager : MonoBehaviour
{
    [SerializeField] float _damageAmount;
    [SerializeField] LayerMask _damageableMask;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_damageableMask.Contains(collision.gameObject.layer))
        {
            //Damageable damageable = collision.gameObject.SearchComponent<Damageable>();
            //damageable.Damage(_damageAmount);
        }
    }
}
