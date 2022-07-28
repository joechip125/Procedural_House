using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;

[Serializable]
public class CubeFacts
{
    public Vector3 location;
    public Color color;
}

public class AreaControl : MonoBehaviour
{
    [SerializeField] private Transform enemyTrans;
    [SerializeField] private float sizeX = 10;
    [SerializeField] private float sizeZ = 10;
    private Rectangle _rectangle;
    private List<CubeFacts> _cubeFacts = new ();

    [SerializeField] private float delayUpdate;
    private float _timeSinceUpdate;

    private Vector3 _min;
    private Vector3 _max;

    private void Awake()
    {
        InitiateList();
    }

    void Start()
    {
        _rectangle = new Rectangle();
    }

    private void InitiateList()
    {
        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                var next = transform.position + new Vector3(i * sizeX,0,j * sizeZ);
                _cubeFacts.Add(new CubeFacts()
                {
                    location = next,
                    color = Color.red
                });
               
            }
        }
        
        var xArray = _cubeFacts.OrderBy(x => x.location.x).ToArray();
        var zArray = _cubeFacts.OrderBy(x => x.location.z).ToArray();

        _max = Vector3.Max(xArray[^1].location, zArray[^1].location);
        _min = Vector3.Min(xArray[0].location, zArray[0].location);
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_timeSinceUpdate > delayUpdate)
        {
            _timeSinceUpdate -= delayUpdate;
            UpdateColors();
        }
        
        _timeSinceUpdate += Time.deltaTime;
    }

    private void UpdateColors()
    {
        var minX = _cubeFacts.OrderBy(x => x.location.x).ToArray()[0].location.x;
        var maxX = _cubeFacts.OrderBy(x => x.location.x).ToArray()[^1].location.x;
        Debug.Log($"first index: {maxX}, /n  last index: {minX}");
    }
    
    private Vector3 GetMinOrMax(bool minOrMax)
    {
        var curr = enemyTrans.position;
        
        
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                var next = transform.position + new Vector3(i * sizeX,0,j * sizeZ);
                Gizmos.DrawWireCube(next, new Vector3(sizeX,3,sizeZ));
                
            }
        }

        if (!Application.isPlaying) return;
        
        foreach (var c in _cubeFacts)
        {
            Gizmos.color = c.color;
            Gizmos.DrawWireCube(c.location, new Vector3(sizeX,3,sizeZ));
        }
    }
}
