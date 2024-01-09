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

        private float rotateSpeed = 20f;
        public Action<int> TargetHit;

        private void Hit(string hitTag)
        {
            int value = 0;
            if (hitTag == "Player")
            {
                value = 1;
            }
            else if (hitTag == "EnemyCommander")
            {
                value = 2;
            }
            TargetHit?.Invoke(value);
        }

        private void Start()
        {
            
        }

        public void Rotation(float rotateDir)
        {
            transform.Rotate(Vector3.up, rotateSpeed * rotateDir);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(shoot))
            {
                Use();
            }
            
            if (Input.GetKey(rotateR))
            {
                transform.Rotate(Vector3.up, -rotateSpeed * Time.deltaTime);
            }
            else if (Input.GetKey(rotateL))
            {
                transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
            }
        }

        public void Use()
        {
            var dir =transform.TransformDirection(Vector3.forward);
            var tempB =Instantiate(bullet, exit.localPosition, Quaternion.identity).GetComponentInChildren<Bullet>();
            tempB.OnTargetHit = Hit;
            tempB.direction = dir;
            tempB.speed = 5f;
        }

        private void OnDrawGizmos()
        {
            var start = exit.position;
            var dir =transform.TransformDirection(Vector3.forward);
            Gizmos.DrawLine(start, start + dir * range);
            Gizmos.DrawSphere(start, 0.1f);
        }
    }
}