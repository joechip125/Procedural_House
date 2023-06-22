using UnityEngine;


public class NewAdvancedMesh_Staircase : NewAdvancedMesh
{
    [SerializeField] private Material aMaterial;
    public void MakeStairs(Vector3 direction, int numSteps)
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        var size = new Vector2(100,30);
        var stepDepth = 40f;
        var stepHeight = 30f;
        var stepWidth = 100f;
        var stepDepth2 = direction * stepDepth / 2;
        var first = new Vector2(stepWidth, stepHeight);
        var second = new Vector2(stepWidth, stepDepth);
        var start = stepDepth * numSteps;
        var third = new Vector2(start, stepHeight);

        var pos = new Vector3(0, size.y / 2, 0);
        
        for (int i = 0; i < numSteps; i++)
        {
            SimplePanel(pos, -direction, first);
            var aCross = Vector3.Cross(Vector3.up, direction);
            SimplePanel(pos + aCross * stepWidth / 2  + direction * start / 2, aCross, third);
            SimplePanel(pos -aCross * stepWidth / 2  + direction * start / 2, -aCross, third);
            pos += Vector3.up * stepHeight / 2 + stepDepth2;
            SimplePanel(pos, Vector3.up, second);
            pos += Vector3.up * stepHeight / 2 + stepDepth2;
            start -= stepDepth;
            third.x -= stepDepth;
        }
    }
}
