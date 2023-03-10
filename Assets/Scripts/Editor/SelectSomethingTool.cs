using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

[EditorTool("Select Something Tool", typeof(TestRoom))]
public class SelectSomethingTool : EditorTool
{
    public override GUIContent toolbarIcon => EditorGUIUtility.IconContent("AvatarPivot");
    
    [Shortcut("Select Something Tool", KeyCode.U)]
    static void PathManipulateShortcut()
    {
        if (Selection.GetFiltered<TestRoom>(SelectionMode.TopLevel).Length < 1) return;
    
        ToolManager.SetActiveTool<SelectSomethingTool>();
    }
    
    public override void OnToolGUI(EditorWindow window)
    {
        if (window is not SceneView) return;

        foreach (var t in targets)
        {
            if(t is not TestRoom room) continue;
            
            EditorGUI.BeginChangeCheck();

            room.SelectSquare();
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(room, "Select Something");
                room.SelectSquare2();
            }
        }
    }
}
