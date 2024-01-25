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

        public FastIKFabricBase(Transform root, Transform target, Transform pole, int chainLength = 2)
        {
            Root = root;
            Pole = pole;
            Target = target;
            Target.position = Root.position;
            var parent = Root.parent;
            Pole.parent = parent;
            Pole.position = parent.position;

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
        
        public void ResolveIK()
        {
            //Fabric

            //  root
            //  (bone0) (bonelen 0) (bone1) (bonelen 1) (bone2)...
            //   x--------------------x--------------------x---...

            //get position
            for (int i = 0; i < Bones.Length; i++)
            {
                Positions[i] = GetPositionRootSpace(Bones[i]);
            }

            var targetPosition = GetPositionRootSpace(Target);
            var targetRotation = GetRotationRootSpace(Target);

            //1st is possible to reach?
            if ((targetPosition - GetPositionRootSpace(Bones[0])).sqrMagnitude >= CompleteLength * CompleteLength)
            {
                //just strech it
                var direction = (targetPosition - Positions[0]).normalized;
                //set everything after root
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
                    //back
                    for (int i = Positions.Length - 1; i > 0; i--)
                    {
                        if (i == Positions.Length - 1)
                            Positions[i] = targetPosition; //set it to target
                        else
                            Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i]; //set in line on distance
                    }

                    //forward
                    for (int i = 1; i < Positions.Length; i++)
                        Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1];

                    //close enough?
                    if ((Positions[^1] - targetPosition).sqrMagnitude < Delta * Delta)
                        break;
                }
            }

            //move towards pole
            
            var polePosition = GetPositionRootSpace(Pole);
            for (int i = 1; i < Positions.Length - 1; i++)
            {
                var plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(polePosition);
                var projectedBone = plane.ClosestPointOnPlane(Positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - Positions[i - 1], projectedPole - Positions[i - 1], plane.normal);
                Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
            }
            
            //set position & rotation
            for (int i = 0; i < Positions.Length; i++)
            {
                if (i == Positions.Length - 1)
                {
                    SetRotationRootSpace(Bones[i], Quaternion.Inverse(targetRotation) * StartRotationTarget * Quaternion.Inverse(StartRotationBone[i]), i);
                }
                
                else
                {
                    SetRotationRootSpace(Bones[i], Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) *
                                                   Quaternion.Inverse(StartRotationBone[i]), i);
                }
                SetPositionRootSpace(Bones[i], Positions[i]);
            }
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