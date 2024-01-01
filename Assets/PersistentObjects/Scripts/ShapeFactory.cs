using UnityEngine;

namespace PersistentObjects.Scripts
{
    [CreateAssetMenu]
    public class ShapeFactory : ScriptableObject
    {
        [SerializeField]
        Shape[] prefabs;

        public Shape Get(int shapeID)
        {
            return Instantiate(prefabs[shapeID]);
        }
        
        public Shape GetRandom () 
        {
            return Get(Random.Range(0, prefabs.Length));
        }
    }
}