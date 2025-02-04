using UnityEngine;

public class SpellBullet : MonoBehaviour
{
    float _speed;
    float _lifeTime;

    Rigidbody2D _rigidBody;
    AttackScriptableObject _attack;
    LayerMask _damageableMask;
    float timePassed;
    [SerializeField] GameObject sfxExplosion;
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _damageableMask = gameObject.layer;
    }

    public void Initialize(LayerMask target, AttackScriptableObject attack, int objectLayer, Vector3 direction)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        _attack = attack;
        _lifeTime = attack.bulletLifeTime;
        _speed = attack.bulletFixedSpeed;
        _damageableMask = target;

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
            if (damageable != null)
                Damager.GiveDamage(damageable, _attack, transform, 0);

        }
        else if (collision.gameObject.layer == 17)
            return;

        Destroy(Instantiate(sfxExplosion, transform.position, Quaternion.identity), 1f);
        Destroy(gameObject);

    }

}
