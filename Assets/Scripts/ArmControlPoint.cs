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
    [SerializeField] private float speed = 2;
    private float _currentExtent;

    private MathParabola _parabola = new MathParabola();

    private float _startHeight;
    
    void Start()
    {
        _startHeight = transform.position.y;
    }

    private void Incrementer()
    {
        if (_plusMinus) _timer += Time.deltaTime * speed;
        else _timer -= Time.deltaTime * speed;
        
        if (_timer is >= 1 or <= -1)
        {
            if (_timer >= 1) _timer = 1;
            if (_timer <= -1) _timer = -1;
            
            _plusMinus = !_plusMinus;
        }
    }

    private void Oscillate()
    {
        var forward = mainTransform.forward;
        var right = mainTransform.right;
        var mainPos = mainTransform.position;
        mainPos.y = _startHeight;

       // var max = mainPos + new Vector3(0, 0, forward.z * 0.3f);
       // var min = mainPos + new Vector3(0, 0, forward.z * -0.5f);
       // var curr = _parabola.Parabola(min, max, 0, _timer);

        if (cosOrSin)
        {
            mainPos +=  new Vector3(right.x * -0.3f, 0, 0);   
            _currentExtent =  Mathf.Sin( (_timer  * MathF.PI / 180))* -maxExtent;
            Debug.Log(Mathf.Cos( (_timer * 180) * MathF.PI / 180));
            Debug.Log(_timer);
        }
        else
        {
            mainPos +=  new Vector3(right.x * 0.3f, 0, 0);
            _currentExtent =  Mathf.Sin( _timer * MathF.PI / 180)* maxExtent;
        }

        var somasf = _currentExtent + transform.position.z;
        somasf = Mathf.Clamp(somasf, mainPos.z - maxExtent, mainPos.z + maxExtent);
       //Debug.Log( mainPos + new Vector3(0,0, _currentExtent));
       // Debug.Log(_timer);
        var pos = transform.position;

        transform.position = mainPos + new Vector3(0,0, _currentExtent);
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
