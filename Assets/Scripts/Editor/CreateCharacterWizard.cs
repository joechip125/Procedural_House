using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreateCharacterWizard : ScriptableWizard
{
    public Texture2D portraitTexture;
    public string nickname = "Default";
    public Color color = Color.cyan;

    [MenuItem("My Tools/ Create Character Wizard...")]
    public static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CreateCharacterWizard>("Create Character",
            "Create New", "Update Selected");
    }

    private void OnWizardCreate()
    {
        var characterGo = new GameObject();
        var character = characterGo.AddComponent<Character>();
        character.portrait = portraitTexture;
        character.nickname = nickname;
        character.color = color;
    }

    private void OnWizardOtherButton()
    {
        if (Selection.activeTransform == null) return;

        var characterComp = Selection.activeTransform.GetComponent<Character>();

        if (characterComp == null) return;
        
        characterComp.portrait = portraitTexture;
        characterComp.nickname = nickname;
        characterComp.color = color;
    }

    private void OnWizardUpdate()
    {
        helpString = "Enter Character Details";
    }
}