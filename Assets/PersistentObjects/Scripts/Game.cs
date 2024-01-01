using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PersistentObjects.Scripts
{
    public class Game : PersistableObject
    {
        
        public PersistentStorage storage;
        
        const int saveVersion = 1;
        
        public ShapeFactory shapeFactory;

        public KeyCode createKey = KeyCode.C;
        public KeyCode newGameKey = KeyCode.N;
        public KeyCode saveKey = KeyCode.S;
        public KeyCode loadKey = KeyCode.L;
        
        private List<Shape> shapes;
        private string savePath;

        private void Awake()
        {
            shapes = new List<Shape>();
            savePath = Path.Combine(Application.persistentDataPath, "saveFile");
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(createKey))
            {
                CreateObject();
            }
            else if (Input.GetKeyDown(newGameKey))
            {
                BeginNewGame();
            }
            else if (Input.GetKeyDown(saveKey))
            {
                storage.Save(this, saveVersion);
            }
            else if (Input.GetKeyDown(loadKey))
            {
                BeginNewGame();
                storage.Load(this);
            }
        }
        
        public override void Save (GameDataWriter writer) 
        {
            writer.Write(shapes.Count);
            for (int i = 0; i < shapes.Count; i++) 
            {
                writer.Write(shapes[i].ShapeID);
                writer.Write(shapes[i].MaterialId);
                shapes[i].Save(writer);
            }
        }
        
        
        public override void Load (GameDataReader reader)
        {
            int version = reader.Version;
            if (version > saveVersion) 
            {
                Debug.LogError("Unsupported future save version " + version);
                return;
            }
            int count = version <= 0 ? -version : reader.ReadInt();
            
            for (int i = 0; i < count; i++) 
            {
                int shapeId = version > 0 ? reader.ReadInt() : 0;
                int materialId = version > 0 ? reader.ReadInt() : 0;
                var instance = shapeFactory.Get(shapeId, materialId);
                instance.Load(reader);
                shapes.Add(instance);
            }
        }
        
        private void BeginNewGame()
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                Destroy(shapes[i].gameObject);
            }
        }

        private void CreateObject()
        {
            var o = shapeFactory.GetRandom();
            var t = o.transform;
            t.localPosition = Random.insideUnitSphere * 5f;
            t.localRotation = Random.rotation;
            t.localScale = Vector3.one * Random.Range(0.1f, 1f);
            shapes.Add(o);
        }
    }
}