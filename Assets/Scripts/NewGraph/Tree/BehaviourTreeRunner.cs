using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour, IEnemyCommands
{
    public BehaviourTree tree;
 //   public Dictionary<STATE, BehaviourTree>  trees;

    void Start()
    {
        tree = tree.Clone();
        tree.Bind(new AiAgent()
        {
            enemyTransform = transform,
            enemyEyes = GetComponentInChildren<TracerEyes>()
        });
    }

    public Transform GetTopParent(Transform pTrans)
    {
        if (pTrans.parent)
        {
            pTrans = pTrans.parent;
            GetTopParent(pTrans);
        }
        
        return pTrans;
    }
    
    void Update()
    {
        tree.Update();
    }

    public void MoveToDestination(Vector3 destination)
    {
        throw new System.NotImplementedException();
    }
}

