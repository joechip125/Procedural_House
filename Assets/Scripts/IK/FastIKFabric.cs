using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FastIKFabric : MonoBehaviour
{
    public int chainLength = 2;

    public Transform target;
    public Transform pole;

    [Header("Solver Parameters")] public int iterations = 30;

    public float delta;

    [Range(0, 1)] public float snapBackStrength = 1f;


    protected float[] BonesLength;
    protected float CompleteLength;
    protected Transform[] Bones;
    protected Vector3[] Positions;
    
    void Start()
    {
        
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        Bones = new Transform[chainLength + 1];
        Positions = new Vector3[chainLength + 1];
        BonesLength = new float[chainLength];

        CompleteLength = 0;

        var current = transform;

        for (var i = Bones.Length - 1; i >= 0; i--)
        {
            Bones[i] = current;

            if (i == Bones.Length - 1)
            {
                
            }
            else
            {
                BonesLength[i] = (Bones[i + 1].position - current.position).magnitude;
                CompleteLength += BonesLength[i];
            }
            
            current = transform.parent;
        }
    }


    private void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (target == default) return;
        
        if(Bones.Length != chainLength)
            Init();

        for (var i = 0; i < Bones.Length; i++)
        {
            Positions[i] = Bones[i].position;
        }

        if ((target.position - Bones[0].position).sqrMagnitude >= CompleteLength * CompleteLength)
        {
            var direction = (target.position - Positions[0]).normalized;

            for (int i = 1; i < Positions.Length; i++)
            {
                Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
            }
        }
        
        for (var i = 0; i < Positions.Length; i++)
        {
            Bones[i].position = Positions[i];
        }
    }

    void Update()
    {
        
    }
}
