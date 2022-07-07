using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu]
public class BehaviourTree : ScriptableObject
{
    public BaseNode rootNode;
    
    public BaseNode.State state = BaseNode.State.Update;

    public BaseNode.State Update()
    {
        return rootNode.Update();
    }
}
