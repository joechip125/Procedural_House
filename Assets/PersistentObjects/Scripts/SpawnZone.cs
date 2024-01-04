﻿using UnityEngine;

namespace PersistentObjects.Scripts
{
    public class SpawnZone : MonoBehaviour
    {
        [SerializeField]
        bool surfaceOnly;
        
        
        public Vector3 SpawnPoint 
        {
            get 
            {
                return transform.TransformPoint(surfaceOnly ? Random.onUnitSphere : Random.insideUnitSphere);
            }
        }
        
        void OnDrawGizmos () 
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(Vector3.zero, 1f);
        }
    }
}