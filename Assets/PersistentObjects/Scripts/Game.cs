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
        public KeyCode destroyKey = KeyCode.X;
        
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
                CreateShape();
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
            else if (Input.GetKeyDown(destroyKey))
            {
               DestroyShape();
            }
        }
        
        void DestroyShape () 
        {
            if (shapes.Count > 0) 
            {
                int index = Random.Range(0, shapes.Count);
                Destroy(shapes[index].gameObject);
                
                int lastIndex = shapes.Count - 1;
                shapes[index] = shapes[lastIndex];
                shapes.RemoveAt(lastIndex);
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

        private void CreateShape()
        {
            var instance = shapeFactory.GetRandom();
            var t = instance.transform;
            t.localPosition = Random.insideUnitSphere * 5f;
            t.localRotation = Random.rotation;
            t.localScale = Vector3.one * Random.Range(0.1f, 1f);
            instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
            shapes.Add(instance);
        }
    }
}