using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapColliderGeneratior))]
public class TilemapColliderGeneratiorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TilemapColliderGeneratior Component = target as TilemapColliderGeneratior;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("GenerateAtStart"));
        if (Component.GenerateAtStart)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("GenerateAt"));
        }
        else
        {
            if (GUILayout.Button("Generate"))
            {
                Component.GenerateColliders();
            }
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("TilemapsToGenerateColliders"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("DivideOutByTilemaps"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Output"));

        serializedObject.ApplyModifiedProperties();
    }
}
