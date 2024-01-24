using System;
using UnityEngine;

namespace RobotGame.Scripts.Animation
{
    public class KinematicTest : MonoBehaviour
    {
        protected Transform[] Bones;
        [SerializeField]

        private void OnDrawGizmos()
        {
            var pos = transform.position;
            var size = new Vector3(1, 3, 1);
            Gizmos.DrawWireCube(pos, size);
        }
    }
}