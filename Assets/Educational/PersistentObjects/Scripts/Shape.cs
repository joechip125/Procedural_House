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
        public Vector3 AngularVelocity { get; set; }
        MeshRenderer meshRenderer;

        void Awake () 
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private int shapeID;
        public int MaterialId { get; private set; }

        private Color Color;
        
        static int colorPropertyId = Shader.PropertyToID("_Color");
        static MaterialPropertyBlock sharedPropertyBlock;
        
        public void SetMaterial (Material material, int materialId) 
        {
            meshRenderer.material= material;
            MaterialId = materialId;
        }

        void FixedUpdate () 
        {
            transform.Rotate(Vector3.forward * (50f * Time.deltaTime));
        }
        
        public void SetColor (Color color)
        {
            Color = color;
            sharedPropertyBlock ??= new MaterialPropertyBlock();
            sharedPropertyBlock.SetColor(colorPropertyId, color);
            meshRenderer.SetPropertyBlock(sharedPropertyBlock);
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