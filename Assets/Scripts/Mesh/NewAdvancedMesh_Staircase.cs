using UnityEngine;


public class NewAdvancedMesh_Staircase : NewAdvancedMesh
{
    [SerializeField] private Material aMaterial;
    public void MakeStairs(Vector3 direction, int numSteps)
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        
        var stepSize = new Vector3(100,30, 40);
        var depthAmount = direction * stepSize.z / 2;
        var upAmount = Vector3.up * stepSize.y / 2;
        var first = new Vector2(stepSize.x, stepSize.y);
        var second = new Vector2(stepSize.x, stepSize.z);
        var third = new Vector2(stepSize.z * numSteps, stepSize.y);
        var pos = new Vector3(0, stepSize.y / 2, 0);
        
        for (int i = 0; i < numSteps; i++)
        {
            SimplePanel(pos, -direction, first);
            var aCross = Vector3.Cross(Vector3.up, direction);
            SimplePanel(pos + aCross * stepSize.x / 2  + direction * third.x / 2, aCross, third);
            SimplePanel(pos -aCross * stepSize.x / 2  + direction * third.x / 2, -aCross, third);
            pos += upAmount + depthAmount;
            SimplePanel(pos, Vector3.up, second);
            pos += upAmount + depthAmount;
            third.x -= stepSize.z;
        }
    }
}
