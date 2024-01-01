using System.IO;
using UnityEngine;

namespace PersistentObjects.Scripts
{
    public class GameDataWriter
    {
        private BinaryWriter writer;

        public GameDataWriter(BinaryWriter binaryWriter)
        {
            
        }

        public void Write (float value) 
        {
            writer.Write(value);
        }

        public void Write (int value) 
        {
            writer.Write(value);
        }

        public void Write (Quaternion value) 
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
            writer.Write(value.w);
        }

        public void Write (Vector3 value) 
        {
            writer.Write(value.x);
            writer.Write(value.y);
            writer.Write(value.z);
        }
    }
}