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
    }
}