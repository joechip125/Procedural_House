using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class OtherFabricIK : MonoBehaviour
{
    [SerializeField] private int numberBones;
    
    // How many times to run the algorithm
    int iterations = 10;

    // How close the affector needs to be to the target for us to stop doing iterations
    float accuracy = 0.001f;
    
    // Target the affector will attempt to move to
    [SerializeField]
    Transform target;

    // List of bones
    Transform[] Bones;

    // List of the position offsets of the bones
    Vector3[] Positions;

    // List of the length of the bones
    float[] BoneLengths;

    // Total length of the bones
    float FullLength;
    
    // Rotation information
    Vector3[] StartDirections;
    Quaternion[] StartRotations;
    Quaternion TargetStartRotation;
    
    
    void Start()
    {
        Init();
    }

    private void Init()
    {
        Bones = new Transform[numberBones + 1];
        Positions = new Vector3[numberBones + 1];
        BoneLengths = new float[numberBones];
        
        // Init rotation information
        StartDirections = new Vector3[numberBones + 1]; // NEW initialize bone start directions array
        StartRotations = new Quaternion[numberBones + 1]; // NEW initialize bone start rotations array

        TargetStartRotation = target.rotation; // NEW save target start rotation


        FullLength = 0;

        var curr = transform;
        

        for (int i = Bones.Length - 1; i >= 0; i--)
        {
            Bones[i] = curr;
            StartRotations[i] = curr.rotation; // NEW Store current bone's initial rotation

            if (i == Bones.Length - 1)
            {
                // Affector bone
                StartDirections[i] = target.position - curr.position; // NEW Store the vector from the target position to the effector position
            }
            else 
            {
                // Set the length of this bone equal to the difference of the position of this and the previous bone
                BoneLengths[i] = (Bones[i + 1].position - curr.position).magnitude;
                // Add current bone's length to the total length of the arm
                FullLength += BoneLengths[i];
                
                StartDirections[i] = Bones[i + 1].position - curr.position; // NEW Store the vector pointing from the current bone to its child
            }

            curr = curr.parent;
        }

    }

    private void ResolveIK()
    {
        // Call Init again if the length of the arm has changed. 
        if (BoneLengths.Length != numberBones)
            Init();

        // Get current bone positions for computations
        for (int i = 0; i < Bones.Length; i++)
            Positions[i] = Bones[i].position;

        var sqrDistanceToTarget = (target.position - Bones[0].position).sqrMagnitude;
        
        if (sqrDistanceToTarget >= FullLength * FullLength) 
        {
            // Get the direction towards the target
            var dir = (target.position - Positions[0]).normalized;

            // Distribute bones along the direction towards the target
            for (int i = 1; i < Positions.Length; i++)
                Positions[i] = Positions[i - 1] + dir * BoneLengths[i - 1];

        }
        else
        {
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                //back propagation
                for (int i = Positions.Length - 1; i > 0; i--) 
                {
                    if (i == Positions.Length - 1)
                    {
                        // Just set the effector to the target position
                        Positions[i] = target.position;
                    }
                    else
                    {
                        // Move the current bone to its new position on the line based on its length and the position of the next bone
                        Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BoneLengths[i];
                    }
                }

                //front propagation
                for (int i = 1; i < Positions.Length; i++)
                {
                    // This time set the current bone's position to the position on the line between itself and the previous bone, taking its length into consideration
                    Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BoneLengths[i - 1];
                }

                // Stop iterating if we are close enough according to the accuracy value
                var sqDistance = (Positions[^1] - target.position).sqrMagnitude;
                if (sqDistance < accuracy * accuracy) break;
            }
        }
        
        

        // Set transform positions to the computed new positions
        for (int i = 0; i < Positions.Length; i++)
            Bones[i].position = Positions[i];
        
        for (int i = 0; i < Positions.Length; i++) 
        {
            if (i == Positions.Length - 1)
            {
                var rot = target.rotation * Quaternion.Inverse(TargetStartRotation) * StartRotations[i];
                Debug.Log(rot.eulerAngles);
                
                Bones[i].rotation = target.rotation * Quaternion.Inverse(TargetStartRotation) * StartRotations[i];
            }
            else
            {
               var rot = Quaternion.FromToRotation(StartDirections[i], Positions[i + 1] - Positions[i]) * StartRotations[i];
               Debug.Log(rot.eulerAngles);
                Bones[i].rotation = Quaternion.FromToRotation(StartDirections[i], Positions[i + 1] - Positions[i]) * StartRotations[i];
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        ResolveIK();
    }

    private void OnDrawGizmos()
    {
        var current = transform;

        for (int i = 0; i < numberBones && current && current.parent; i++)
        {
            Debug.DrawLine(current.position, current.parent.position, Color.green);
            current = current.parent;
        }
    }
}
