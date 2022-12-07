using UnityEngine;

public class SpellBullet : MonoBehaviour
{
    [SerializeField] bool isFixedSpeed;
    [SerializeField] float speed;
    [SerializeField] float decelerator;
    [SerializeField] float timeIsAlive;

    Rigidbody2D _rigidBody;
    AttackScriptableObject _attack;
    LayerMask _damageableMask;
    float timePassed;
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(LayerMask target, AttackScriptableObject attack, EDirection rotation, int objectLayer)
    {
        _attack = attack;
        _damageableMask = target;
        gameObject.layer = objectLayer;
        switch (rotation)
        {
            case EDirection.Up:
                transform.LookAt(transform.position + Vector3.forward, Vector3.up);
                break;
            case EDirection.Down:
                transform.LookAt(transform.position + Vector3.forward, Vector3.down);
                break;
            case EDirection.Left:
                transform.LookAt(transform.position + Vector3.forward, Vector3.left);
                break;
            case EDirection.Right:
                transform.LookAt(transform.position + Vector3.forward, Vector3.right);
                break;
        }
  
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed >= timeIsAlive)
            Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if(isFixedSpeed)
        {
            _rigidBody.velocity = speed * Time.deltaTime * transform.up;
        }
        else
        {
            _rigidBody.velocity = speed * Time.deltaTime * transform.up;
            speed -= decelerator;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_damageableMask.Contains(collision.gameObject.layer))
        {
            Damageable damageable = collision.gameObject.SearchComponent<Damageable>();
            if(damageable != null)
                Damager.GiveDamage(damageable, _attack, transform);
        }

        Destroy(gameObject);
    }
}
