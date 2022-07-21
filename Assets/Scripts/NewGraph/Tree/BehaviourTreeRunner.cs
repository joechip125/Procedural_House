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
        tree.Bind(new AiAgent()
        {
            enemyTransform = transform,
            enemyEyes = GetComponentInChildren<TracerEyes>()
        });
    }
    
    void Update()
    {
        tree.Update();
    }
}

