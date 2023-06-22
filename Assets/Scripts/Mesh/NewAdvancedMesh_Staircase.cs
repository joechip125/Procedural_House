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

        var pos = new Vector3(0, size.y / 2, 0);
        
        for (int i = 0; i < numSteps; i++)
        {
            SimplePanel(pos, -direction, first);
            pos += Vector3.up * stepHeight / 2 + stepDepth2;
            SimplePanel(pos, Vector3.up, second);
            pos += Vector3.up * stepHeight / 2 + stepDepth2;
        }
    }
}
