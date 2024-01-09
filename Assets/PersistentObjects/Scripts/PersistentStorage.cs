using System.IO;
using UnityEngine;

namespace PersistentObjects.Scripts
{
    public class PersistentStorage : MonoBehaviour
    {
        string savePath;

        void Awake () 
        {
            savePath = Path.Combine(Application.persistentDataPath, "saveFile");
        }

        public void Save (PersistableObject o, int version)
        {
            using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))) 
            {
                writer.Write(-version);
                o.Save(new GameDataWriter(writer));
            }
        }

        public void Load (PersistableObject o) 
        {
            var data = File.ReadAllBytes(savePath);
            var reader = new BinaryReader(new MemoryStream(data));
            o.Load(new GameDataReader(reader, -reader.ReadInt32()));
            
            //using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open))) 
            //{
            //    o.Load(new GameDataReader(reader, -reader.ReadInt32()));
            //}
        }
    }
}