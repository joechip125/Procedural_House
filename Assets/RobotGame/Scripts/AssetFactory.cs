using System.Collections.Generic;
using PersistentObjects.Scripts;
using UnityEngine;

namespace RobotGame.Scripts
{
    [CreateAssetMenu(fileName = "AssetFactory", menuName = "Robot Game", order = 0)]
    public class AssetFactory : ScriptableObject
    {
        [SerializeField] 
        private PersistentObject[] prefabs;
        
        [SerializeField]
        bool recycle;
        
        List<PersistentObject>[] pools;

        void CreatePools ()
        {
            pools = new List<PersistentObject>[prefabs.Length];
            for (int i = 0; i < pools.Length; i++) 
            {
                pools[i] = new List<PersistentObject>();
            }
        }
        
        public void Reclaim (PersistentObject shapeToRecycle) 
        {
            if (recycle) 
            {
                if (pools == null) 
                {
                    CreatePools();
                }
                pools[shapeToRecycle.assetID].Add(shapeToRecycle);
                shapeToRecycle.gameObject.SetActive(false);
            }
            else 
            {
                Destroy(shapeToRecycle.gameObject);
            }
        }
        
        public PersistentObject Get (int shapeId = 0, int materialId = 0) 
        {
            PersistentObject instance;
            if (recycle) 
            {
                if (pools == null) 
                {
                    CreatePools();
                }
                List<PersistentObject> pool = pools[shapeId];
                int lastIndex = pool.Count - 1;
                if (lastIndex >= 0) 
                {
                    instance = pool[lastIndex];
                    instance.gameObject.SetActive(true);
                    pool.RemoveAt(lastIndex);
                }
            }
            
            instance = Instantiate(prefabs[shapeId]);
            instance.assetID = shapeId;
            return instance;
        }
    }
}