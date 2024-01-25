using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RobotGame.Scripts.IK
{
    [Serializable]
    public class IKLimb
    {
        public IKBone[] bones;
        
        public int ChainLength;
        
        public Transform Target;
        public Transform Pole;
        public Transform Root;
        public Transform Leaf;
        
        public IKLimb(Transform leaf, Transform target, Transform pole, int chainLength = 2)
        {
            bones = new IKBone[chainLength + 1];
            Target = target;
            Pole = pole;
            Leaf = leaf;
            
            
            Init();
        }

        private void Init()
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