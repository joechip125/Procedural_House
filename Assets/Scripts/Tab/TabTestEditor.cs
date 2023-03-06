using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TabTest))]
public class TabTestEditor : Editor
{
    private TabTest myTarget;
    
    private readonly string[] buttons = new[] { "Button1", "Button2", "Button3", "Button4" };

    private void OnEnable()
    {
        myTarget = (TabTest)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        myTarget.currentTab = GUILayout.Toolbar(myTarget.currentTab, buttons);
    }
}
