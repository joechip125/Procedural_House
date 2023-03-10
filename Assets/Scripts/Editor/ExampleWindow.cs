using System;
using UnityEditor;
using UnityEngine;

public class ExampleWindow : EditorWindow
{
    [MenuItem("My Tools/ Open Example Window")]
    public static void ShowWindow()
    {
        GetWindow<ExampleWindow>();
    }
    
    private void OnGUI()
    {
        
    }
}
