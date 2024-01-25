using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RobotGame.Scripts.IK
{
    [Serializable]
    public class FastIKFabricBase
    {
        public int ChainLength;
        
        public Transform Target;
        public Transform Pole;
        public Transform Root;
        
        [Header("Solver Parameters")]
        public int Iterations = 10;

        public float Delta = 0.001f;
        
        [Range(0, 1)]
        public float SnapBackStrength = 1f;

        public float[] BonesLength; //Target to Origin
        public float CompleteLength;
        public Transform[] Bones;
        public Vector3[] Positions;
        public Vector3[] StartDirectionSucc;
        public Quaternion[] StartRotationBone;
        public Quaternion StartRotationTarget;

        public FastIKFabricBase(Transform leaf, Transform target, Transform pole, int chainLength = 2)
        {
            Root = leaf;
            Pole = pole;
            Target = target;
            Target.position = Root.position + Vector3.right * 0.05f;
            
            Pole.parent =  Root.parent;
            Pole.position = Pole.parent.position;

            ChainLength = chainLength;
            Init();
        }

        private void Init()
        {
            Bones = new Transform[ChainLength + 1];
            Positions = new Vector3[ChainLength + 1];
            BonesLength = new float[ChainLength];
            StartDirectionSucc = new Vector3[ChainLength + 1];
            StartRotationBone = new Quaternion[ChainLength + 1];
            
            for (var i = 0; i <= Bones.Length - 1; i++)
            {
                if (Root.parent == null) continue;
                    Root = Root.parent;
            }

            StartRotationTarget = GetRotationRootSpace(Target);

            var current = Root;
            CompleteLength = 0;
            for (var i = Bones.Length - 1; i >= 0; i--)
            {
                Bones[i] = current;
                StartRotationBone[i] = GetRotationRootSpace(current);
            
                if (i == Bones.Length - 1)
                {
                    StartDirectionSucc[i] = GetPositionRootSpace(Target) - GetPositionRootSpace(current);
                }
                else
                {
                    StartDirectionSucc[i] = GetPositionRootSpace(Bones[i + 1]) - GetPositionRootSpace(current);
                    BonesLength[i] = StartDirectionSucc[i].magnitude;
                    CompleteLength += BonesLength[i];
                }
            
                current = current.parent;
            }
        }

        public void ResolveTest()
        {
            for (int i = 0; i < Bones.Length; i++)
            {
                Positions[i] = GetPositionRootSpace(Bones[i]);
            }    
        }
        
        public void ResolveIK()
        {
            for (int i = 0; i < Bones.Length; i++)
            {
                Positions[i] = GetPositionRootSpace(Bones[i]);
            }

            var targetPosition = GetPositionRootSpace(Target);
            var targetRotation = GetRotationRootSpace(Target);
            
            if ((targetPosition - GetPositionRootSpace(Bones[0])).sqrMagnitude >= CompleteLength * CompleteLength)
            {
                var direction = (targetPosition - Positions[0]).normalized;
                for (int i = 1; i < Positions.Length; i++)
                {
                    Positions[i] = Positions[i - 1] + direction * BonesLength[i - 1];
                }
            }
            else
            {
                for (int i = 0; i < Positions.Length - 1; i++)
                {
                    Positions[i + 1] = Vector3.Lerp(Positions[i + 1], Positions[i] + StartDirectionSucc[i],
                        SnapBackStrength);
                }
                
                for (int iteration = 0; iteration < Iterations; iteration++)
                {
                    for (int i = Positions.Length - 1; i > 0; i--)
                    {
                        if (i == Positions.Length - 1)
                            Positions[i] = targetPosition;
                        else
                            Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i]; //set in line on distance
                    }
                    for (int i = 1; i < Positions.Length; i++)
                        Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1];
                    
                    if ((Positions[^1] - targetPosition).sqrMagnitude < Delta * Delta)
                        break;
                }
            }

            var polePosition = GetPositionRootSpace(Pole);
            for (int i = 1; i < Positions.Length - 1; i++)
            {
                var plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(polePosition);
                var projectedBone = plane.ClosestPointOnPlane(Positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - Positions[i - 1], projectedPole - Positions[i - 1], plane.normal);
                Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
            }
            
            for (int i = 0; i < Positions.Length; i++)
            {
                if (i == Positions.Length - 1)
                {
                    SetRotationRootSpace(Bones[i], 
                        Quaternion.Inverse(targetRotation) * StartRotationTarget * 
                        Quaternion.Inverse(StartRotationBone[i]), i);
                }
                
                else SetRotationRootSpace(Bones[i], 
                    Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) *
                    Quaternion.Inverse(StartRotationBone[i]), i);
                
                SetPositionRootSpace(Bones[i], Positions[i]);
            }
        }

        public Vector3 GetPositionAtIndex(int index)
        {
            return GetPositionRootSpace(Bones[index]);
        }
        
        private Vector3 GetPositionRootSpace(Transform current)
        {
            return Quaternion.Inverse(Root.rotation) * (current.position - Root.position);
        }

        private void SetPositionRootSpace(Transform current, Vector3 position)
        {
            current.position = Root.rotation * position + Root.position;
        }

        private Quaternion GetRotationRootSpace(Transform current)
        {
            return Quaternion.Inverse(current.rotation) * Root.rotation;
        }

        private void SetRotationRootSpace(Transform current, Quaternion rotation, int index)
        {
            var theRot = Root.rotation * rotation;
            current.rotation = theRot;
        }
    }
}