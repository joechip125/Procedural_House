using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour
{
    [SerializeField] private Transform mainTarget;
    [SerializeField] private Transform leftLeg;
    [SerializeField] private Transform rightLeg;
    private float _counter;
    private bool _toggle;
    private Vector3 _leftSave;
    private Vector3 _rightSave;
    
    
    void Start()
    {
        
    }
    
    void Update()
    {
        var forward = transform.forward;
        var right = transform.right;
        transform.position += forward * (Time.deltaTime * 0.3f);

        _counter += Time.deltaTime;
        if (_counter > 1f)
        {
            if (_toggle)
            {
                var dir = forward + -right;
                leftLeg.position = transform.position + new Vector3(dir.x * 0.3f, 0, dir.z * 0.3f);
                _leftSave = leftLeg.position;
            }
            else
            {
                var dir = forward + right;
                rightLeg.position = transform.position + new Vector3(dir.x * 0.3f, 0, dir.z * 0.3f);
                _rightSave = rightLeg.position;
            }

            _toggle = !_toggle;
            _counter -= 1f;
        }

        leftLeg.position = _leftSave;
        rightLeg.position = _rightSave;
        
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * (Time.deltaTime * 1);
            _counter += Time.deltaTime;
            if (_counter > 1f)
            {
                if (_toggle)
                {
                    leftLeg.position += leftLeg.forward * 0.4f;
                }
                else
                {
                    rightLeg.position += rightLeg.forward * 0.4f;
                }

                _toggle = !_toggle;
                _counter -= 0.3f;
            }
        }
    }
}
