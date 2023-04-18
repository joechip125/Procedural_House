using System;
using UnityEngine;

public class NewAdvancedMesh_Door : NewAdvancedMesh
{
    [Range(-1, 1)]public float directionX;
    [Range(-1, 1)]public float directionZ;
    [SerializeField] private Vector3 wallDirection;
    public float sizeX;
    public float sizeZ;

    [SerializeField] private float startAxis;
    private readonly Vector3[] corners = new[]
    {   new Vector3(-1, 0, -1), 
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), 
        new Vector3(1, 0, 0) };
    
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
        
        DoPanel(pos + new Vector3(0,0, 50), aCross, new Vector3(100,100,10));
    }

    private void DoPanel(Vector3 position, Vector3 inverseNormal, Vector3 aSize)
    {
        Vector3 aCrossForward = Vector3.Cross(inverseNormal, Vector3.up).normalized;
        Vector3 aCrossUp = Vector3.Cross(aCrossForward, inverseNormal).normalized;
        
        var start = startAxis;

        if (wallDirection.y != 0 && wallDirection.x + wallDirection.z == 0)
        {
            aCrossForward = Vector3.Cross(inverseNormal, Vector3.forward).normalized;       
        }

        Vector3 sumCross = (aCrossForward + aCrossUp).normalized;
        
        for (var i = 0; i < 4; i++)
        {
            var amount = Mathf.Sqrt(Mathf.Pow(aSize.x, 2) + Mathf.Pow(aSize.z, 2)) / 2;
            var betterAngle = Quaternion.AngleAxis(start, inverseNormal) *sumCross;
            Vector3 newSpot = position + (betterAngle.normalized * amount);
            var total = position +  Vector3.Scale((aSize / 2), betterAngle);

            corners[i] = newSpot;
            start += -90f;
        }
        lastVert = AddQuad(corners[0], corners[1], corners[2], corners[3]);
    }

    private void OnDrawGizmos()
    {
        var dir = new Vector3(directionX, 0, directionZ);
        var pos = transform.position + new Vector3(0, 50,0);
        var aCross = Vector3.Cross(dir, Vector3.up);
        var aCross2 = Vector3.Cross(dir, Vector3.down);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + dir * 50);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + aCross * 50);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pos, pos + aCross2 * 50);

        foreach (var c in corners)
        {
            var aPos = new Vector3(c.x, 0, c.z);
            Gizmos.DrawSphere(aPos, 3);
        }
    }
}
