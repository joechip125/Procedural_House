using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmControlPoint : MonoBehaviour
{
    [SerializeField] private Transform controlTarget;
    [SerializeField] private Transform mainTransform;
    [SerializeField] private bool cosOrSin;
    [SerializeField] private float maxExtent;
    private float _timer ;
    private bool _plusMinus = true;
    private float _speed = 1;
    private float _currentExtent;

    private MathParabola _parabola = new MathParabola();

    private float _startHeight;
    
    void Start()
    {
        _startHeight = transform.position.y;
    }

    private void Incrementer()
    {
        if (_plusMinus) _timer += Time.deltaTime * _speed;
        else _timer -= Time.deltaTime * _speed;
        
        if (_timer is >= 1 or <= 0)
        {
            if (_timer >= 1) _timer = 1;
            if (_timer <= 0) _timer = 0;
            
            _plusMinus = !_plusMinus;
        }
        Debug.Log(_timer);
    }

    private void Oscillate()
    {
        var forward = mainTransform.forward;
        var right = mainTransform.right;
        var mainPos = mainTransform.position+ new Vector3(right.x * 0.3f, 0, forward.z * 0.3f);
        mainPos.y = _startHeight;

        var max = mainPos + new Vector3(0, 0, forward.z * 0.3f);
        var min = mainPos + new Vector3(0, 0, forward.z * -0.5f);
        var curr = _parabola.Parabola(min, max, 0, _timer);

        if (cosOrSin)
        {
            _currentExtent =  Mathf.Cos( _timer * MathF.PI / 180)* maxExtent;
        }
        else
        {
            _currentExtent =  Mathf.Sin( _timer * MathF.PI / 180)* maxExtent;
        }

        var somasf = _currentExtent + transform.position.z;
        somasf = Mathf.Clamp(somasf, mainPos.z - maxExtent, mainPos.z + maxExtent);
        Debug.Log(curr);
        var pos = transform.position;

        transform.position = curr;
    }

    void Update()
    {
        Incrementer();
        Oscillate();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
        if (controlTarget)
        {
          //  Gizmos.DrawSphere(controlTarget.position, 0.1f);
        }
    }
#endif
}
