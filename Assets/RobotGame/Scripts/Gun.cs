using System;
using System.Collections.Generic;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class Gun : PersistentObject, IComponent
    {

        [SerializeField] 
        private Transform exit;

        [SerializeField] 
        private GameObject bullet;

        [SerializeField] 
        private KeyCode shoot = KeyCode.C;
        [SerializeField] 
        private KeyCode rotateR = KeyCode.Z;
        [SerializeField] 
        private KeyCode rotateL = KeyCode.X;

        [SerializeField, Range(1, 100)] 
        private float range;

        
        private void Hit(string hitTag)
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(shoot))
            {
                Use();
            }
            
            if (Input.GetKey(rotateR))
            {
                
            }
            else if (Input.GetKey(rotateL))
            {
                
            }
        }

        public void Use()
        {
            var dir =transform.TransformDirection(Vector3.forward);
            var tempB =Instantiate(bullet, exit.localPosition, Quaternion.identity, transform).GetComponent<Bullet>();
            tempB.OnTargetHit = Hit;
        }

        private void OnDrawGizmos()
        {
            var start = exit.position;
            var dir =transform.TransformDirection(Vector3.forward);
            Gizmos.DrawLine(start, start + dir * range);
        }
    }
}