using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private float x;
    private float y;
    private Vector3 rotateValue;
    public Camera theCamera;
    public float speed = 30;
    private Vector3 aDirection;

    private void Awake()
    {
        theCamera = Camera.main;
        //theCamera.transform.
    }
    
    // Update is called once per frame
    void Update()
    {
        y = Input.GetAxis("Mouse X");
        x = Input.GetAxis("Mouse Y");
        var anAngle = transform.eulerAngles;
        Vector3 forward = Quaternion.Euler(anAngle) * Vector3.forward;
        Vector3 right = Quaternion.Euler(anAngle) * Vector3.right;
        var dir = theCamera.transform.InverseTransformDirection(Vector3.forward);
        aDirection = dir;

        Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += forward * (Time.deltaTime * speed);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            transform.position += -forward * (Time.deltaTime * speed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += -right * (Time.deltaTime * speed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += right * (Time.deltaTime * speed);
        }

        rotateValue = new Vector3(x, y * -1, 0);
        transform.eulerAngles -= rotateValue;
    }
    
}
