using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class BehaviorTreeView : GraphView
{
    private BehaviourTree _tree;
    
    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits>
    {
        
    }

    public BehaviorTreeView()
    {
        Insert(0, new GridBackground());
        
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }

    internal void PopulateView(BehaviourTree tree)
    {
        _tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
        
        _tree.nodes.ForEach(CreateNodeView);
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        graphViewChange.elementsToRemove?.ForEach(x =>
        {
            if (x is NodeView nodeView)
            {
                _tree.DeleteNode(nodeView.Node);
            }
        });
        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var t in types)
            {
                evt.menu.AppendAction($"[{t.BaseType.Name}] {t.Name}", a => CreateNode(t));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var t in types)
            {
                evt.menu.AppendAction($"[{t.BaseType.Name}] {t.Name}", a => CreateNode(t));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var t in types)
            {
                evt.menu.AppendAction($"[{t.BaseType.Name}] {t.Name}", a => CreateNode(t));
            }
        }

    }

    private void CreateNode(System.Type type)
    {
        var node = _tree.CreateNode(type);
        CreateNodeView(node);
    }

    private void CreateNodeView(BaseNode node)
    {
        var nodeView = new NodeView(node);
        AddElement(nodeView);
    }
}
