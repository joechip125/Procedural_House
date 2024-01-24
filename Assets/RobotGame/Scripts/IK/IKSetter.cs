using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobotGame.Scripts.IK
{
    public class IKSetter : MonoBehaviour
    {
        private List<Transform> leafNodes;
        //Name
        //Transform of handle
        private bool quit;
        private const string rootName = "Root";

        [SerializeField] 
        private List<string> nameOfLeafNodes;
        
        [SerializeField] 
        private GameObject handle;
        
        private void Start()
        {
            quit = false;
            leafNodes = new List<Transform>();

            leafNodes = transform
                .Cast<Transform>()
                .ToArray()
                .Where(x => nameOfLeafNodes
                    .Contains(x.name))
                .ToList();
        }
    }
}