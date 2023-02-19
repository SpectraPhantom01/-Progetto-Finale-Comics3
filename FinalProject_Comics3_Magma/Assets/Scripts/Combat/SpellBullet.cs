using UnityEngine;

public class SpellBullet : MonoBehaviour
{
    float _speed;
    float _lifeTime;

    Rigidbody2D _rigidBody;
    AttackScriptableObject _attack;
    LayerMask _damageableMask;
    float timePassed;
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(LayerMask target, AttackScriptableObject attack, EDirection rotation, int objectLayer, Vector3 direction)
    {
        _attack = attack;
        _lifeTime = attack.bulletLifeTime;
        _speed = attack.bulletFixedSpeed;
        _damageableMask = target;
        gameObject.layer = objectLayer;

        transform.LookAt(transform.position + Vector3.forward, direction);
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= _lifeTime)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        _rigidBody.velocity = _speed * Time.deltaTime * transform.up;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_damageableMask.Contains(collision.gameObject.layer))
        {
            Damageable damageable = collision.gameObject.SearchComponent<Damageable>();
            if(damageable != null)
                Damager.GiveDamage(damageable, _attack, transform, 0);
        }

        Destroy(gameObject);
    }
}
