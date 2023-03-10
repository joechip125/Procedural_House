using System;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public class ExampleWindow : EditorWindow
{
    public Color color;
    
    [MenuItem("My Tools/ Colorize")]
    public static void ShowWindow()
    {
        GetWindow<ExampleWindow>("Colorize");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Color the selected objects", EditorStyles.boldLabel);

        color = EditorGUILayout.ColorField("The Color", color);
        if (GUILayout.Button("Colorize"))
        {
            foreach (var g in Selection.gameObjects)
            {
                var rend = g.GetComponent<Renderer>();
                
                if(Equals(rend, null)) continue;

                rend.sharedMaterial.color = color;
            }
        }
    }
}
