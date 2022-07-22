using System;
using UnityEngine;
public class MoveTest : MonoBehaviour
{
    [SerializeField] Transform lookAtTarget;


    private void Update()
    {
        if (!lookAtTarget) return;

        transform.LookAt(lookAtTarget.position);
    }
}
