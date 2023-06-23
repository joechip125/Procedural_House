using System;
using UnityEngine;

public class NewAdvancedMesh_Door : NewAdvancedMesh
{
    [Range(-1, 1)]public float directionX;
    [Range(-1, 1)]public float directionZ;
    [SerializeField] private Vector3 wallDirection;
    public float sizeX;
    public float sizeZ;
    
    private int lastVert;
    public Material aMaterial;
    
    
    private void Awake()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        
        var dir = new Vector3(directionX, 0, directionZ);
        var pos = transform.position + new Vector3(0, 50,0);
        var aCross = Vector3.Cross(dir, Vector3.up);
        var aCross2 = Vector3.Cross(dir, Vector3.down);
       
    }

    
    
}
