using UnityEngine;

namespace PersistentObjects.Scripts
{
    public abstract class SpawnZone : PersistableObject
    {
        public abstract Vector3 SpawnPoint { get; }
        
    }
}