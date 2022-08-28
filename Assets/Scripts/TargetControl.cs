using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetControl : MonoBehaviour
{
    private Vector3 _startPos;
    void Start()
    {
        _startPos = transform.position;
    }
    
    void Update()
    {
        transform.position = _startPos;
    }
}
