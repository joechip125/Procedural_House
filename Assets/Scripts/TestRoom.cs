using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AdvancedMesh))]
[ExecuteInEditMode]
public class TestRoom : MonoBehaviour
{
    private AdvancedMesh mesh;
    [SerializeField, Range(10, 1000)] private float sizeX;
    [SerializeField, Range(10, 1000)] private float sizeZ;
    [SerializeField, Range(10, 300)] private float sizeY;
    private Vector3[] corners;
    private Color color = Color.white;
    //[HideInInspector] List<Vector3>


    private void Awake()
    {
        if (!Application.isEditor) return;
        mesh = GetComponent<AdvancedMesh>();
        corners = new Vector3[4];
        mesh.InstanceMesh();
    }

    private void Start()
    {
        if (!Application.isPlaying) return;
        Debug.Log("Game started");
    }

    private void SetCorners()
    {
        var pos = transform.position;
        corners[0] = new Vector3(pos.x - sizeX / 2, pos.y, pos.z - sizeZ / 2);
        corners[1] = new Vector3(pos.x - sizeX / 2, pos.y, pos.z + sizeZ / 2);
        corners[2] = new Vector3(pos.x + sizeX / 2, pos.y, pos.z + sizeZ / 2);
        corners[3] = new Vector3(pos.x + sizeX / 2, pos.y, pos.z - sizeZ / 2);
    }

    public void SelectSquare()
    {
        color = Color.green;
    }
    
    public void SelectSquare2()
    {
        color = Color.white;
    }
    
    
    private void OnDrawGizmos()
    {
        SetCorners();
        var pos = transform.position;
        var add = new Vector3(0, sizeY, 0);
        Gizmos.color = Color.gray;
        Gizmos.DrawCube(pos, new Vector3(sizeX, 1, sizeZ));
        
        for (int i = 0; i < 4; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(corners[i], 4);
            Gizmos.DrawSphere(corners[i] + add, 4);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(corners[i], corners[i] + add);

            if (i < 3)
            {
                Gizmos.DrawLine(corners[i] + add, corners[i + 1] + add);
            }
            else
            {
                Gizmos.DrawLine(corners[i] + add, corners[0] + add);
            }
        }
        
    }
}
