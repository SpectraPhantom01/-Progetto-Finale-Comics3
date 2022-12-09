using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAliveEntity
{
    public List<AttackScriptableObject> AttackList { get; }
    public bool IsAlive { get; set; }
    public string Name { get; }

    public void Kill();
}
