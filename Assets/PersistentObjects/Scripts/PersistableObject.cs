using UnityEngine;

namespace PersistentObjects.Scripts
{
    public class PersistableObject : MonoBehaviour
    {
        public void Save (GameDataWriter writer) 
        {
            writer.Write(transform.localPosition);
            writer.Write(transform.localRotation);
            writer.Write(transform.localScale);
        }

        public void Load (GameDataReader reader) 
        {
            transform.localPosition = reader.ReadVector3();
            transform.localRotation = reader.ReadQuaternion();
            transform.localScale = reader.ReadVector3();
        }
    }
}