using System;
using System.Collections.Generic;
using System.Linq;
using RobotGame.Scripts.Animation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

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

        public Spline spline = new();

        public BezierKnot startKnot;
        public BezierKnot endKnot;
        
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


        private void SplineTest()
        {
            var temp = spline;
            var startPos = leafNodes.Count > 1 ? leafNodes[2].position: transform.position;
            startKnot = new BezierKnot()
            {
                Position = startPos
            };
        }
        
        private void TestParabola()
        {
            var startPos = leafNodes.Count > 1 ? leafNodes[2].position: transform.position;
            parabola ??= new Parabola();
            
            var end = startPos + Vector3.forward * 1f;
            var time = 0f;
            var interval = 1f  / numberSamples;
            for (int i = 0; i < numberSamples; i++)
            {
                var current = MathHelpers.Parabola(startPos, end, 0.5f, time);
                time += interval;
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(current, 0.01f);
            }
            
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPos, 0.1f);
            Gizmos.DrawSphere(end, 0.05f);
        }
        
        
        private void OnDrawGizmos()
        {
           TestParabola();
        }
    }
}