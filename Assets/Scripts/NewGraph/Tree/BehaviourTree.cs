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
}
