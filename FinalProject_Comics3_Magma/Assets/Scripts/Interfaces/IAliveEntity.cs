using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAliveEntity
{
    public bool IsAlive { get; set; }
    public string Name { get; set; }

    public void Kill();
}
