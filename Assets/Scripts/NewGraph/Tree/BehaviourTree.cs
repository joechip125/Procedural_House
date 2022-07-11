using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu]
public class BehaviourTree : ScriptableObject
{
    public BaseNode rootNode;
    
    public BaseNode.State state = BaseNode.State.Update;

    public List<BaseNode> nodes = new();

    public BaseNode.State Update()
    {
        if (state == BaseNode.State.Update)
        {
            state = rootNode.Update();
        }

        return state;
    }

    public BaseNode CreateNode(System.Type type)
    {
        var node = ScriptableObject.CreateInstance(type) as BaseNode;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        nodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        
        return node;
    }

    public void DeleteNode(BaseNode node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(BaseNode parent, BaseNode child)
    {
        var decorator = parent as DecoratorNode;

        if (decorator)
        {
            decorator.child = child;
        }

        var root = parent as RootNode;

        if (root)
        {
            root.child = child;
        }
        
        var composite = parent as CompositeNode;

        if (composite)
        {
            composite.children.Add(child);
        }
    }
    public void RemoveChild(BaseNode parent, BaseNode child)
    {
        var decorator = parent as DecoratorNode;

        if (decorator)
        {
            decorator.child = null;
        }

        var root = parent as RootNode;

        if (root)
        {
            root.child = null;
        }
        
        
        var composite = parent as CompositeNode;

        if (composite)
        {
            composite.children.Remove(child);
        }
    }
    
    public List<BaseNode> GetChildren(BaseNode targetNode)
    {
        var decorator = targetNode as DecoratorNode;
        var theChildren = new List<BaseNode>();

        if (decorator && decorator.child)
        {
            theChildren.Add(decorator.child);
        }

        var root = targetNode as RootNode;

        if (root && root.child)
        {
            theChildren.Add(root.child);
        }
        
        var composite = targetNode as CompositeNode;

        if (composite)
        {
            return composite.children;
        }

        return theChildren;
    }

    public BehaviourTree Clone()
    {
        var tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        return tree;
    }
    
}
