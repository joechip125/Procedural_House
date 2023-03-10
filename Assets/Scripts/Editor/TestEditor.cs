using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestRoom))]
public class TestEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Something"))
        {
            
        }
        if (GUILayout.Button("Something Else"))
        {
            
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
}
