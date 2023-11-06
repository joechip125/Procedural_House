using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixTest : MonoBehaviour
{
    private List<Vector3> things = new();
    public int points;
    public float length;
    
    private void OnValidate()
    {
        for (int i = 0; i < points; i++)
        {
            var ratio = (float)i / (float)points;

            ratio *= 2 * Mathf.PI;
        }
    }
}
