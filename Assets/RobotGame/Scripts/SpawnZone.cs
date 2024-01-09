using UnityEngine;

namespace RobotGame.Scripts
{
    public abstract class SpawnZone : PersistentObject
    {
        public abstract Vector3 SpawnPoint { get; }
    }
}