using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits>
    {
        
    }

    private Editor _editor;
    
    public InspectorView()
    {
        
    }

    public void UpdateSelection(NodeView nodeView)
    {
       Clear();
       _editor = Editor.CreateEditor(nodeView.Node);

       var container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
       Add(container);
    }
}
