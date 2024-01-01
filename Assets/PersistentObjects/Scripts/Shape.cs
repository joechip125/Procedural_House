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
        public int MaterialId { get; private set; }
        
        public void SetMaterial (Material material, int materialId) 
        {
            GetComponent<MeshRenderer>().material = material;
            MaterialId = materialId;
        }
        
        private int shapeID;
    }
}