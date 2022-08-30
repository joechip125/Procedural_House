using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControlPoint : MonoBehaviour
{
    [SerializeField] private Transform controlTarget;
    [SerializeField] private bool cosOrSin;
    [SerializeField] private float maxExtent;
    private float _timer;
    private bool _plusMinus;
    private float _speed = 20;
    private float _currentExtent;
    
    void Start()
    {
        
    }

    private void Incrementer()
    {
        if (_plusMinus)
        {
            _timer += Time.deltaTime * _speed;
        }
        else
        {
            _timer -= Time.deltaTime * _speed;
        }

        if (_timer is >= 180 or <= -180)
        {
            _plusMinus = !_plusMinus;
        }
    }
    
    void Update()
    {
        Incrementer();

        if (cosOrSin)
        {
            _currentExtent =  Mathf.Cos( _timer * MathF.PI / 180) * maxExtent;
        }
        else
        {
            _currentExtent =  Mathf.Sin( _timer * MathF.PI / 180) * maxExtent;
        }
    }
}
