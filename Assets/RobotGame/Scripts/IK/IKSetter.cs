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
        [SerializeField, Range(-1, 1)]
        private int xVal = 0;
        [SerializeField, Range(-1, 1)]
        private int yVal = 0;
        [SerializeField, Range(-1, 1)]
        private int zVal = 0;
        
        
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
           // parabola.GetSamples(pos, );
        }
        
        
        private void OnDrawGizmos()
        {
           TestParabola();
        }
    }
}