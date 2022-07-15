using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotationLimiter : MonoBehaviour
{
    public Vector2 xMinMax;
    public Vector2 yMinMax;
    public Vector2 zMinMax;
    
    public void SetRotation(Quaternion rotation)
    {
        var x = Mathf.Clamp(rotation.x, xMinMax.x, xMinMax.y);
        var y = Mathf.Clamp(rotation.y, yMinMax.x, yMinMax.y);
        var z = Mathf.Clamp(rotation.z, zMinMax.x, zMinMax.y);
        Debug.Log(x);
        Debug.Log(y);
        Debug.Log(z);

        transform.rotation = new Quaternion(x, y, z, 0);
    }
    
}
