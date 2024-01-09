using UnityEngine;

namespace PersistentObjects.Scripts
{
    public class CompositeSpawnZone : SpawnZone
    {
        [SerializeField]
        SpawnZone[] spawnZones;

        [SerializeField] 
        private bool sequential;

        private int nextSequentialIndex;
        
        public override Vector3 SpawnPoint 
        {
            get
            {
                int index;
                if (sequential)
                {
                    index = nextSequentialIndex++;
                    if (nextSequentialIndex >= spawnZones.Length) 
                    {
                        nextSequentialIndex = 0;
                    }
                }
                else
                {
                    index = Random.Range(0, spawnZones.Length);    
                }
                
                return spawnZones[index].SpawnPoint;
            }
        }
    }
}