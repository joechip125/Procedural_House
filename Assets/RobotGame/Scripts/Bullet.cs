﻿using System;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class Bullet : PersistentObject
    {
        public Vector3 direction;
        public float speed;
        public Action<string> OnTargetHit;
        private bool hit;

        private float lifeSpan = 10;

        private float timeAlive = 0;
        
        private void Update()
        {
            if (hit) return;
            timeAlive += Time.deltaTime;
            if (timeAlive >= lifeSpan)
            {
                gameObject.SetActive(false);
            }
            transform.localPosition += direction * (speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hit) return;
            OnTargetHit?.Invoke(collision.transform.tag);
            gameObject.SetActive(false);
            hit = true;
        }
    }
}