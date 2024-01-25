using System;
using UnityEngine;

namespace RobotGame.Scripts.IK
{
    [Serializable]
    public struct IKBone
    {
        public Transform Bone;
        public Vector3 StartDirection;
        public Quaternion StartRotation;
        public float BoneLength;

        public IKBone(Transform bone, Vector3 startDirection, Quaternion startRotation)
        {
            Bone = bone;
            StartRotation = startRotation;
            StartDirection = startDirection;
            BoneLength = 0;
        }
    }
}