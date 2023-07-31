using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SaveSO.asset", menuName = "ScriptableObject/Save Asset")]
public class SaveSO : ScriptableObject
{
    public string SceneName;
    public Vector3 LastCheckPointPosition = Vector3.zero;
}
