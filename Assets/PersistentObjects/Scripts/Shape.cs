using UnityEngine;

namespace PersistentObjects.Scripts
{
    public class Shape : PersistableObject
    {
        public int ShapeID
        {
            get => shapeID;
            set
            {
                if(shapeID == int.MinValue && value != int.MinValue) shapeID = value;
                else
                {
                    Debug.LogError("Not allowed to change shapeId.");
                }
            }
        }

        private int shapeID;
        public int MaterialId { get; private set; }

        private Color Color;
        
        public void SetMaterial (Material material, int materialId) 
        {
            GetComponent<MeshRenderer>().material = material;
            MaterialId = materialId;
        }

        public void SetColor (Color color)
        {
            Color = color;
            GetComponent<MeshRenderer>().material.color = color;
        }
        
        public override void Save (GameDataWriter writer) 
        {
            base.Save(writer);
            writer.Write(Color);
        }

        public override void Load (GameDataReader reader) 
        {
            base.Load(reader);
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
    }
}