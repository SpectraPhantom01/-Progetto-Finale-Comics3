using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] List<Hourglass> hourglasses;

    public float CurrentTimeLife => _currentTimeLife;

    private float _currentTimeLife;
    private IAliveEntity _entity;
    private Hourglass currentHourglass;
    private void Start()
    {
        if (hourglasses == null || hourglasses.Count == 0)
            throw new ArgumentNullException($"La lista delle clessidre è vuota!!! {gameObject.name}");


        currentHourglass = hourglasses.First();
        _currentTimeLife = currentHourglass.Time;
        _entity = gameObject.SearchComponent<IAliveEntity>();
    }

    public void Damage(float amount)
    {
        _currentTimeLife -= amount;
        Debug.Log($"Got damage!!! TIME LIFE: {_currentTimeLife}/{currentHourglass.Time}");

        if (_currentTimeLife <= 0)
        {
            hourglasses.Remove(currentHourglass);

            currentHourglass = hourglasses.First();
            if(currentHourglass == null)
            {
                Debug.Log($"This damageable is death: {gameObject.name}");
                _entity.Kill();
                return;
            }

            _currentTimeLife = currentHourglass.Time;
        }
    }

    public void Heal(float amount)
    {
        if (_currentTimeLife + amount >= currentHourglass.Time)
        {
            _currentTimeLife = currentHourglass.Time;
            Debug.Log($"This damageable has full TIME life: {gameObject.name}");
            return;
        }

        _currentTimeLife += amount;
        Debug.Log($"Got heal!!! TIME LIFE: {_currentTimeLife}/{currentHourglass.Time}");

    }
}
