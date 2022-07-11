using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
    
    void Start()
    {
        tree = tree.Clone();
    }
    
    void Update()
    {
        tree.Update();
    }
}

