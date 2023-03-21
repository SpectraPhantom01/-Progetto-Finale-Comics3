using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float launchForce;
    [SerializeField] float bombTime;
    [SerializeField, Range(0f, 1f)] float parabolic;
    [SerializeField] GameObject areaEffect;
    [SerializeField] float destroyAreaAfterSpawn;
    Rigidbody2D _rigidbody;
    float timePassed;
    bool used = false;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector3 direction)
    {
        if (direction.x != 0)
            direction.y = parabolic;
        else
            _rigidbody.gravityScale = 0;

        _rigidbody.AddForce(direction * launchForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
        timePassed += Time.deltaTime;
        if(timePassed >= bombTime && !used)
        {
            if(areaEffect != null)
            {
                var vfx =Instantiate(areaEffect, transform.position, Quaternion.identity);
                Destroy(vfx, destroyAreaAfterSpawn);
            }

            Destroy(gameObject);
            used = true;
        }
    }
}
