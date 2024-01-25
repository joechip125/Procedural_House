using System;
using System.Collections.Generic;
using System.Linq;
using RobotGame.Scripts.Animation;
using Unity.VisualScripting;
using UnityEngine;

namespace RobotGame.Scripts.IK
{
    public class IKSetter : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> leafNodes;
        
        [SerializeField] 
        private GameObject handle;

        public List<FastIKFabricBase> limbs = new();

        private Parabola parabola;

        [SerializeField, Range(3, 50)]
        private int numberSamples = 12;

        [SerializeField]
        public Vector3 gravity;
        
        
        
        private void Start()
        {
            SetIK();
        
        }

        private void SetIK()
        {
            foreach (var leaf in leafNodes)
            {
                limbs.Add(new FastIKFabricBase(leaf, 
                    Instantiate(handle).transform, 
                    Instantiate(handle).transform));
            }
        }

        private void LateUpdate()
        {
            foreach (var limb in limbs)
            {
                limb.ResolveIK();
            }
        }

        private void TestRoot()
        {
            foreach (var limb in limbs)
            {
                for (int i = 0; i < limb.Positions.Length; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(limb.Bones[i].position, 0.1f);
                }
            }
        }

        private void TestParabola()
        {
            var startPos = limbs.Count > 1 ? limbs[2].Bones[^1].position: transform.position;
            parabola ??= new Parabola();
            parabola.GetSamples(startPos, startPos + Vector3.forward * 2f, 
                gravity, numberSamples);
            var end = startPos + Vector3.forward * 2f;
            var time = 0f;
            var interval = 1f  / numberSamples;
            for (int i = 0; i < numberSamples; i++)
            {
                var current = MathHelpers.Parabola(startPos, end, 0.5f, time);
                time += interval;
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(current, 0.05f);
            }
            ;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPos, 0.1f);
        }
        
        
        private void OnDrawGizmos()
        {
           TestParabola();
        }
    }
}