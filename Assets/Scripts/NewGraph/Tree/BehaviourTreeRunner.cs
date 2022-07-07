using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    private BehaviourTree tree;
    
    void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviourTree>();
        
        var log1 = ScriptableObject.CreateInstance<DebugLogNode>();
        log1.message = "Hello Nobody";
        var pause1 = ScriptableObject.CreateInstance<WaitNode>();
        pause1.duration = 3;
        
        var log2 = ScriptableObject.CreateInstance<DebugLogNode>();
        log2.message = "Hello Somebody";
        var pause2 = ScriptableObject.CreateInstance<WaitNode>();
        pause2.duration = 5;
        
        var log3 = ScriptableObject.CreateInstance<DebugLogNode>();
        log3.message = "Hello Everybody";
        var pause3 = ScriptableObject.CreateInstance<WaitNode>();
        pause3.duration = 7;
        
        var sequence = ScriptableObject.CreateInstance<SequencerNode>();
        sequence.children.Add(log1);
        sequence.children.Add(pause1);
        sequence.children.Add(log2);
        sequence.children.Add(pause2);
        sequence.children.Add(log3);
        sequence.children.Add(pause3);

        var repeat = ScriptableObject.CreateInstance<RepeatNode>();
        repeat.child = sequence;


        tree.rootNode = repeat;
    }
    
    void Update()
    {
        tree.Update();
    }
}

