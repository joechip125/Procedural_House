using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeView : Node
{
    public BaseNode Node;
    
    public NodeView(BaseNode node)
    {
        Node = node;
        this.title = node.name;
    }

    public sealed override string title
    {
        get { return base.title; }
        set { base.title = value; }
    }
}
