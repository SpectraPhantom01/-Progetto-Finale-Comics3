using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    
    public void Save()
    {
        Publisher.Publish(new SaveMessage());
    }
    public void Load()
    {
        Publisher.Publish(new LoadMessage());
    }
}
