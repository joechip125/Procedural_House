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
        [SerializeField, Range(-1f, 1f)]
        private int xVal;
        [SerializeField, Range(-1f, 1f)]
        private int yVal;
        [SerializeField, Range(-1f, 1f)]
        private int zVal;
        
        [Header("Parabola Acceleration")]
        [SerializeField, Range(-1f, 1f)]
        private int xVal1;
        [SerializeField, Range(-1f, 1f)]
        private int yVal1;
        [SerializeField, Range(-1f, 1f)]
        private int zVal1;
        
        
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
            var pos = transform.position;
            parabola ??= new Parabola();
            parabola.GetSamples(pos, new Vector3(xVal,yVal, zVal), 
                new Vector3(xVal1,yVal1, zVal1), numberSamples);
        }
        
        
        private void OnDrawGizmos()
        {
           TestParabola();
        }
    }
}