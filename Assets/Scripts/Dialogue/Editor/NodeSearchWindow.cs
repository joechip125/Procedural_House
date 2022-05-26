using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView _graphView;
    private EditorWindow window;
    private Texture2D indentationIcon;

    public void Init(EditorWindow editorWindow, DialogueGraphView graphView)
    {
        _graphView = graphView;
        window = editorWindow;
        indentationIcon = new Texture2D(1, 1);
        indentationIcon.SetPixel(0,0, new Color(0,0,0,0));
        indentationIcon.Apply();
    }
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue Node"), 1),
            new SearchTreeEntry(new GUIContent("Dialogue Node", indentationIcon))
            {
                userData = new DialogueNode(), level = 2
            }
        };
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent,
            context.screenMousePosition - window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);
            
        switch (SearchTreeEntry.userData)
        {
            case DialogueNode dialogueNode:
                _graphView.CreateNode("Dialogue Node", localMousePosition);
                return true;
                
            default:
                return false;
        }
    }
}
