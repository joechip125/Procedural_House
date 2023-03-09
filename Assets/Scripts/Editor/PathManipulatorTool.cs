using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.ShortcutManagement;
using UnityEngine;

[EditorTool("Path Manipulator Tool", typeof(MeshPath))]
public class PathManipulatorTool : EditorTool
{
    public override GUIContent toolbarIcon => EditorGUIUtility.IconContent("AvatarPivot");

    [Shortcut("Path Manipulator Tool", KeyCode.U)]
    static void PathManipulateShortcut()
    {
        if (Selection.GetFiltered<MeshPath>(SelectionMode.TopLevel).Length < 1) return;
        
        ToolManager.SetActiveTool<PathManipulatorTool>();
    }
    
    public override void OnToolGUI(EditorWindow window)
    {
        if (window is not SceneView) return;

        foreach (var t in targets)
        {
            if(t is not MeshPath path) continue;

            for (var i = 0; i < path.vertPos.Count; i++)
            {
                var point = path.vertPos[i];
                
                EditorGUI.BeginChangeCheck();

                point = Handles.PositionHandle(point, Quaternion.identity);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(path, "Moved Control Point");
                    path.SetControlPoint(i, point);
                }
            }
        }
    }
}
