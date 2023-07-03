using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="New Logs", menuName = "ScriptableObject/Logs")]
public class LogScriptableObject : ScriptableObject
{
    public List<string> Logs;

    public void Clear() => Logs = new();
}
