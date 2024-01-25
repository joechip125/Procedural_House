using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RobotGame.Scripts.IK
{
    [Serializable]
    public class IKLimb
    {
        public IKBone[] bones;
        
        public Transform Target;
        public Transform Pole;
        public Transform Root;
        public Transform Leaf;
        private Quaternion StartRotationTarget;

        public IKLimb(Transform leaf, Transform target, Transform pole, int chainLength = 2)
        {
            bones = new IKBone[chainLength + 1];
            Target = target;
            Pole = pole;
            Leaf = leaf;
            
            
            Initialize();
        }

        private void Initialize()
        {
            Root = Leaf;
            var bone = new IKBone();
            for (var i = 0; i <= bones.Length - 1; i++)
            {
                bones[i] = new IKBone();
                if (Root.parent == null)
                {
                    break;
                }
                else
                {
                    
                }
                Root = Root.parent;
            }
        }
        
        private void Init()
        {
            var current = Root;
            
            for (var i = 0; i <= bones.Length - 1; i++)
            {
                if (Root.parent == null) continue;
                Root = Root.parent;
            }

            StartRotationTarget = GetRotationRootSpace(Target);

            var CompleteLength = 0;
            for (var i = bones.Length - 1; i >= 0; i--)
            {
                bones[i].Bone = current;
                bones[i].StartRotation = GetRotationRootSpace(current);
            
                if (i == bones.Length - 1)
                {
                    bones[i].StartDirection = GetPositionRootSpace(Target) - GetPositionRootSpace(current);
                }
                else
                {
                    bones[i].StartDirection = GetPositionRootSpace(bones[i + 1].Bone) - GetPositionRootSpace(current);
                    BonesLength[i] = bones[i].StartDirection.magnitude;
                    CompleteLength += BonesLength[i];
                }
            
                current = current.parent;
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

        private void SetRotationRootSpace(Transform current, Quaternion rotation)
        {
            var theRot = Root.rotation * rotation;
            current.rotation = theRot;
        }
    }
}