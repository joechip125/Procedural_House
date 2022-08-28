using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathParabola
{
    public Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        float Func(float x) => -4 * height * x * x + 4 * height * x;
        var mid = Vector3.Lerp(start, end, t);
        return new Vector3(mid.x, Func(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
}
