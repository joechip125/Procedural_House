using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyPoint : MonoBehaviour
{
    [SerializeField] private GameObject Spawnable;
    [SerializeField] private int numberToSpawn;
    
    void Start()
    {
        SpawnStuff();
        GetComponentsInChildren<MeshRenderer>()[^1].transform.parent.gameObject.SetActive(false);
    }


    private void SpawnStuff()
    {
        var some = GetComponentInChildren<MeshRenderer>().bounds;
        var objSize = Spawnable.GetComponentInChildren<MeshRenderer>().bounds.size;
        var firstPos = new Vector3(some.min.x, some.max.y, some.min.z) + objSize / 2;

        var maxX = some.size.x / objSize.x;
        var maxZ = some.size.z / objSize.z;
        
        for (var i = 0; i < maxZ; i++)
        {
            for (var j = 0; j < maxX; j++)
            {
                if (numberToSpawn < 1) break;
                var next = firstPos + new Vector3(objSize.x * j,0,objSize.z * i);
            
                Instantiate(Spawnable, next, Quaternion.identity, transform);
                
                numberToSpawn--;
            }
        }
    }
    
}
