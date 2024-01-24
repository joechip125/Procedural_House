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

        private List<FastIKFabricBase> limbs = new();
        
        private void Start()
        {
            SetIK();  
        }

        private void SetIK()
        {
            for (int i = 0; i < leafNodes.Count; i++)
            {
                limbs.Add(new FastIKFabricBase(leafNodes[i], 
                    Instantiate(handle).transform, 
                    Instantiate(handle).transform));
                
                
            }
            
            foreach (var leaf in leafNodes)
            {
                limbs.Add(new FastIKFabricBase(leaf, 
                    Instantiate(handle).transform, 
                    Instantiate(handle).transform));
            }
        }

        private void Update()
        {
            foreach (var limb in limbs)
            {
                limb.ResolveIK();
            }
        }
        

        private void OnDrawGizmos()
        {
            for (int i = 0; i < leafNodes.Count; i++)
            {
                Gizmos.DrawWireSphere(leafNodes[i].position, 0.1f);
            }
        }
    }
}