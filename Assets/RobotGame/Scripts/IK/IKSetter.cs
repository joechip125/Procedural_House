using System;
using System.Collections.Generic;
using System.Linq;
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
                limb.ResolveTest();
            }
        }

        private void TestRoot()
        {
            foreach (var limb in limbs)
            {
                for (int i = 0; i < limb.Positions.Length; i++)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(limb.Positions[i], 0.1f);
                    
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(limb.Bones[i].position, 0.1f);
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            TestRoot();
        }
    }
}