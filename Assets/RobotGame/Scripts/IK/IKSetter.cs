using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobotGame.Scripts.IK
{

    [Serializable]
    public struct ControlNode
    {
        public ControlNode(string theName)
        {
            
        }
    }

    public class IKSetter : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> leafNodes;
        //Name
        //Transform of handle
        private bool quit;
        private const string rootName = "Root";
        
        [SerializeField] 
        private GameObject handle;
        
        private void Start()
        {
          
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
            if (!Application.isPlaying) return;
            
            for (int i = 0; i < leafNodes.Count; i++)
            {
                Gizmos.DrawWireSphere(leafNodes[i].position, 0.3f);
            }
        }
    }
}