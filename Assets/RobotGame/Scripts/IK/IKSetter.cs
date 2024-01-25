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

        private Parabola parabola = new();

        [SerializeField, Range(3, 50)]
        private int numberSamples = 12;
        
        [Header("Parabola Velocity")]
        [SerializeField, Range(-1.0f, 1.0f)]
        private float xVal;
        [SerializeField, Range(-1.0f, 1.0f)]
        private float yVal;
        [SerializeField, Range(-1.0f, 1.0f)]
        private float zVal;
        
        [Header("Parabola Acceleration")]
        [SerializeField, Range(-1.0f, 1.0f)]
        private float xVal1;
        [SerializeField, Range(-1.0f, 1.0f)]
        private float yVal1;
        [SerializeField, Range(-1.0f, 1.0f)]
        private float zVal1;
        
        
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
            var pos = limbs.Count > 1 ? limbs[2].Bones[^1].position: transform.position;
            parabola ??= new Parabola();
            parabola.GetSamples(pos, new Vector3(xVal,yVal, zVal), 
                new Vector3(xVal1,yVal1, zVal1), numberSamples);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(pos, 0.1f);
            
            foreach (var t in parabola.samplePositions)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(t, 0.01f);
            }
        }
        
        
        private void OnDrawGizmos()
        {
           TestParabola();
        }
    }
}