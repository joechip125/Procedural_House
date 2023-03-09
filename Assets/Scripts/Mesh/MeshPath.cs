using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

[ExecuteInEditMode]
public class MeshPath : MonoBehaviour
{
    private AdvancedMesh mesh;
    public Material aMaterial;
    public List<Vector3> points = new ();
    private Matrix4x4 aMatrix;
    [SerializeField] public List<Vector3> vertPos = new();
    
    private void Awake()
    {
        if (!Application.isEditor) return;
        mesh = gameObject.GetComponent<AdvancedMesh>() == null ? 
            gameObject.AddComponent<AdvancedMesh>() : gameObject.GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        mesh.ApplyMaterial(aMaterial);
    }


    public void SetControlPoint(int pointIndex, Vector3 position)
    {
        if (pointIndex >= vertPos.Count) return;

        vertPos[pointIndex] = position;
    }
    
    private void SetPoints(Vector3 direction, Vector3 size, Vector3 startPoint, bool backFront = false)
    {
        points.Clear();

        if(backFront) points.Add(startPoint);
        else points.Add(startPoint + Vector3.Scale(direction, size));

        if (direction.z != 0)
        {
            points.Add(startPoint + new Vector3(0, 0, size.z * direction.z));
        }
        
        if (direction.x != 0)
        {
            points.Add(startPoint + new Vector3(size.x * direction.x, 0,0));
        }

        if (direction.y != 0)
        {
            points.Add(startPoint + new Vector3(0, size.y,0));
        }

        if(!backFront) points.Add(startPoint);
        else points.Add(startPoint + Vector3.Scale(direction, size));
    }
    
    private void OnDrawGizmos () 
    {
        if (vertPos.Count < 1) return;
        
        Gizmos.color = Color.black;
        
        for (int i = 0; i < vertPos.Count; i++) 
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(vertPos[i], 0.1f);
            if (i > 0)
            {
                var diff = vertPos[i] - vertPos[i - 1];
                var start = vertPos[i - 1] + diff / 2;

                Gizmos.color = Color.white;
                Gizmos.DrawCube(start, new Vector3(2,2,2));
                
                Gizmos.color = Color.red;
                var prev = vertPos[i - 1];
                Gizmos.DrawLine(prev, vertPos[i]);
            }
        }
    }
}
