using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    public BehaviourTree tree;
    public Transform commanderTrans;
    public bool readyToRun;
    
    void Start()
    {
        Setup();
    }

    private void Setup()
    {
     
    }
    
    private void OnDisable()
    {
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
        if (!readyToRun) return;
        
        tree.Update();
    }

    public void MoveToDestination(Vector3 destination)
    {
       
    }
    
}

