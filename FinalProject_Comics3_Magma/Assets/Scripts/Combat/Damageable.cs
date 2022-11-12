using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] float _maxLife;
    private float _currentLife;
    private IAliveEntity _entity;
    private void Start()
    {
        _currentLife = _maxLife;
        _entity = gameObject.SearchComponent<IAliveEntity>();
    }

    public void Damage(float amount)
    {
        _currentLife -= amount;
        Debug.Log($"Got damage!!! LIFE: {_currentLife}/{_maxLife}");


        if (_currentLife <= 0)
        {
            Debug.Log($"This damageable is death: {gameObject.name}");
            _entity.Kill();
        }
    }

    public void Heal(float amount)
    {
        if (_currentLife + amount >= _maxLife)
        {
            _currentLife = _maxLife;
            Debug.Log($"This damageable has full life: {gameObject.name}");
            return;
        }

        _currentLife += amount;
        Debug.Log($"Got heal!!! LIFE: {_currentLife}/{_maxLife}");

    }
}
