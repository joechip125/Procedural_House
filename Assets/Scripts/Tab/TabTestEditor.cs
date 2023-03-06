using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Zenject;

[CustomEditor(typeof(TabTest))]
public class TabTestEditor : Editor
{
    private TabTest myTarget;
    private SerializedObject soTarget;
    
    private SerializedProperty stringVar1;

    private SerializedProperty intVar1;
    
    private readonly string[] buttons = new[] { "Strings", "Integers", "Button3", "Button4" };

    private void OnEnable()
    {
        myTarget = (TabTest)target;
        soTarget = new SerializedObject(target);

        stringVar1 = soTarget.FindProperty("stringVar1");
        intVar1 = soTarget.FindProperty("intVar1");
    }

    public override void OnInspectorGUI()
    {
        soTarget.Update();
        ToolbarOne();

        EditorGUI.BeginChangeCheck();
        switch (myTarget.stringTab)
        {
            case "Strings":
                EditorGUILayout.PropertyField(stringVar1);
                break;
            case "Integers":
                EditorGUILayout.PropertyField(intVar1);
                break;
        }
        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
        }
    }

    private void ToolbarOne()
    {
        EditorGUI.BeginChangeCheck();
        
        myTarget.currentTab = GUILayout.Toolbar(myTarget.currentTab, buttons);

        myTarget.stringTab = buttons[myTarget.currentTab];

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }
    }
    
    
}
