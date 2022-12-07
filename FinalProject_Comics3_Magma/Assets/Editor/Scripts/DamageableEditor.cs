using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Damageable))]
public class DamageableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Damageable damageable = (Damageable)target;

        if (damageable.gameObject.SearchComponent<PlayerManager>() != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Player Lock Time");
            damageable.PlayerLockTime = EditorGUILayout.FloatField(damageable.PlayerLockTime);
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enemy Lock Time");
            damageable.EnemyLockTime = EditorGUILayout.FloatField(damageable.EnemyLockTime);
            GUILayout.EndHorizontal();
        }
       
    }
}
