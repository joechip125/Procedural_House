using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : Node
{
    public Action<NodeView> OnNodeSelected;
    
    public BaseNode Node;

    public Port input;
    public Port output;

    
    
    public NodeView(BaseNode node) : base("Assets/NodeView.uxml")
    {
        Node = node;
        title = node.name;

        viewDataKey = Node.guid;
        
        style.left = Node.position.x;
        style.top = Node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        var styleSheet = (StyleSheet) EditorGUIUtility.Load("NodeStyle.uss");
        //  mainContainer.styleSheets.Add(styleSheet);
    }

    private void CreateInputPorts()
    {
      
        switch (Node)
        {
            case ActionNode:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                //mainContainer.style.backgroundColor = new StyleColor(Color.red);
                break;
            case CompositeNode:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool)); 
                //mainContainer.style.backgroundColor = new StyleColor(Color.green);
                break;
            case DecoratorNode:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                //mainContainer.style.backgroundColor = new StyleColor(Color.blue);
                break;
        }
        
        if (input != default)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    private void CreateOutputPorts()
    {
        switch (Node)
        {
            case ActionNode:
                break;
            case CompositeNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
            case DecoratorNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
        }
        
        
        if (output != default)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    public sealed override string title
    {
        get { return base.title; }
        set { base.title = value; }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Node.position.x = newPos.xMin;
        Node.position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();

        OnNodeSelected?.Invoke(this);

    }
    
    
}
