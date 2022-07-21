using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
    
    void Start()
    {
        tree = tree.Clone();
        tree.Bind(new AiAgent());
    }
    
    void Update()
    {
        tree.Update();
    }
}

