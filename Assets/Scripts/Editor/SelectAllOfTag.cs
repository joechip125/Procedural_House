using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectAllOfTag : ScriptableWizard
{
   public string currentTag = "Your Tag here";
   
   [MenuItem("My Tools/ Select All Of Tag...")]
   static void SelectAllOfTagWizard()
   {
      ScriptableWizard.DisplayWizard<SelectAllOfTag>("Select All Of Tag...", "Make Selection");
   }

   private void OnWizardCreate()
   {
      var gameObjects = GameObject.FindGameObjectsWithTag(currentTag);
      Selection.objects = gameObjects;
   }
}
