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
            //if (!collision.gameObject.TryGetComponent<Damageable>(out var damageable))
            //{
            //    damageable = collision.gameObject.GetComponentInChildren<Damageable>();
            //    if (damageable == null)
            //    {
            //        damageable = collision.gameObject.GetComponentInParent<Damageable>();
            //        if (damageable == null)
            //            return;
            //    }
            //}

            //Damageable damageable = collision.gameObject.SearchComponent<Damageable>();
            //damageable.Damage(_damageAmount);
            
        }
    }
}
