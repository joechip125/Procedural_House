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
        //Name
        //Transform of handle
        private bool quit;
       
        
        [SerializeField] 
        private GameObject handle;
        
     //   public List<GameObject> 

        private void Start()
        {
            SetIK();  
        }

        private void SetIK()
        {
            
            foreach (var leaf in leafNodes)
            {
                var target = Instantiate(handle).transform;
                var pole = Instantiate(handle).transform;
                var FastBase = new FastIKFabricBase(leaf, target, pole);
                var fast = leaf.AddComponent<FastIKFabric>();
            }
        }
        
        private void SetLeafNodes()
        {
            var nodes = transform
                .Cast<Transform>()
                .ToArray().ToList();
            //.Where(x => nameOfLeafNodes
            //    .Contains(x.name))
            //.ToList();
        }
        
        private void OnDrawGizmos()
        {
          //  if (!Application.isPlaying) return;
            
            for (int i = 0; i < leafNodes.Count; i++)
            {
                Gizmos.DrawWireSphere(leafNodes[i].position, 0.1f);
            }
        }
    }
}