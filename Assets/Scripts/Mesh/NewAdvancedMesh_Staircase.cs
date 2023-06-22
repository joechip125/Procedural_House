using UnityEngine;


public class NewAdvancedMesh_Staircase : NewAdvancedMesh
{
    [SerializeField] private Material aMaterial;
    public void MakeStairs(Vector3 direction, int numSteps)
    {
        ApplyMaterial(aMaterial);
        
        var pos = transform.position;
        for (int i = 0; i < numSteps; i++)
        {
            SimplePanel(new Vector3(), direction, new Vector2(100,30));
        }
    }
}
