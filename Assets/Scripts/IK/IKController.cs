using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    private OtherFabricIK FabricIK;
    private GameObject target;
    private Transform control;

    private void Awake()
    {
        FabricIK = GetComponent<OtherFabricIK>();
        target = Instantiate(targetObject, transform);
        FabricIK.target = target.transform;
    }

}
