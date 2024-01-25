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
    }
}