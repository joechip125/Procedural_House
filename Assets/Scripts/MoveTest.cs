using System;
using UnityEngine;
public class MoveTest : MonoBehaviour
{
    [SerializeField] Transform lookAtTarget;

    private void Start()
    {
        lookAtTarget.transform.position = transform.position;
        // transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.forward);
        //transform.rotation = Quaternion.FromToRotation(transform.forward, lookAtTarget.position);
    }


    private void Update()
    {
        if (!lookAtTarget) return;

        //transform.rotation = Quaternion.FromToRotation(transform.forward, lookAtTarget.position);
      //  transform.rotation = Quaternion.FromToRotation(Vector3.up, transform.forward);
        transform.LookAt(lookAtTarget.position, Vector3.forward);
    }
}
