using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomTrigger))]
public class RoomTriggerEditor : Editor
{
    List<EnemyController> insideEnemies;
    public override void OnInspectorGUI()
    {
        RoomTrigger room = (RoomTrigger)target;
        PolygonCollider2D collider = room.GetComponent<PolygonCollider2D>();
        
        insideEnemies = room.EnemyControllers;
        
        var style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 12;
        style.fixedHeight = 30;
        style.hover.textColor = new Color(0, 204, 204);

        if (GUILayout.Button("Get Inside Bounds Enemies", style))
        {
            insideEnemies = FindObjectsOfType<EnemyController>().Where(x => collider.bounds.Contains(x.transform.position)).ToList();
            room.EnemyControllers = insideEnemies.ToList();
        }

        if(room.EnemyControllers != null && room.EnemyControllers.Count > 0)
        {
            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField($"Inside Enemies: {insideEnemies.Count}");
            foreach (var enemyController in insideEnemies)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{enemyController.Name}", GUILayout.MinWidth(40), GUILayout.MaxWidth(130));
                EditorGUILayout.ObjectField(enemyController.gameObject, typeof(EnemyController), true, GUILayout.MinWidth(40));
                GUILayout.EndHorizontal();
            }

            EditorUtility.SetDirty(room);
        }

        style.hover.textColor = Color.red;

        if (GUILayout.Button("Clear List", style))
        {
            room.EnemyControllers.Clear();
        }
    }
}
