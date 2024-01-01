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

        public Shape Get(int shapeID)
        {
            var instance = Instantiate(prefabs[shapeID]);
            instance.ShapeID = shapeID;
            return instance;
        }
        
        public Shape GetRandom () 
        {
            return Get(Random.Range(0, prefabs.Length));
        }
    }
}