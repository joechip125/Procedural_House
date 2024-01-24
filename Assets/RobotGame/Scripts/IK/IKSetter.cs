using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobotGame.Scripts.IK
{
    public class IKSetter : MonoBehaviour
    {
        private List<Transform> targets;
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
            targets = new List<Transform>();

            var child = transform.GetChild(0);
            
            var current = transform;
            while (!quit)
            {
                if (current != null)
                {
                    var numChild = current.childCount;

                    if (numChild < 2)
                    {
                        
                        continue;
                    }
                    
                    var children = current
                        .Cast<Transform>()
                        .ToArray();
                    
                    foreach (var c in children)
                    {
                       
                    }

                    for (int i = 0; i < numChild; i++)
                    { 
                        if (child.childCount > 1)
                        {
                           
                        }
                    }

                    child = current.GetChild(0);
                }
                quit = true;
            }
        }
    }
}