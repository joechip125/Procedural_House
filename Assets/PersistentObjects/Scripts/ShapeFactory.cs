﻿using System.Collections.Generic;
using UnityEngine;

namespace PersistentObjects.Scripts
{
    [CreateAssetMenu]
    public class ShapeFactory : ScriptableObject
    {
        [SerializeField] 
        private Shape[] prefabs;
        
        [SerializeField]
        Material[] materials;
        
        [SerializeField]
        bool recycle;
        
        List<Shape>[] pools;

        
        void CreatePools ()
        {
            pools = new List<Shape>[prefabs.Length];
            for (int i = 0; i < pools.Length; i++) 
            {
                pools[i] = new List<Shape>();
            }
        }
        
        public void Reclaim (Shape shapeToRecycle) 
        {
            if (recycle) 
            {
                if (pools == null) 
                {
                    CreatePools();
                }
                pools[shapeToRecycle.ShapeID].Add(shapeToRecycle);
            }
        }
        
        public Shape Get (int shapeId = 0, int materialId = 0) 
        {
            Shape instance;
            if (recycle) 
            {
                if (pools == null) 
                {
                    CreatePools();
                }
                List<Shape> pool = pools[shapeId];
                int lastIndex = pool.Count - 1;
                if (lastIndex >= 0) 
                {
                    instance = pool[lastIndex];
                    instance.gameObject.SetActive(true);
                    pool.RemoveAt(lastIndex);
                }
            }
            
            instance = Instantiate(prefabs[shapeId]);
            instance.ShapeID = shapeId;
            instance.SetMaterial(materials[materialId], materialId);
            return instance;
        }
        
        public Shape GetRandom () 
        {
            return Get(Random.Range(0, prefabs.Length), 
                Random.Range(0, prefabs.Length));
        }
    }
}