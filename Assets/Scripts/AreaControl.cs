using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using Color = UnityEngine.Color;

public enum AssetTypes
{
    Wall,
}

[Serializable]
public class CubeFacts
{
    public Vector3 location;
    public Color color;
    public Vector2 coordinate;
}

[Serializable]
public class ExtraCubes
{
    public Vector3 location;
    public Color color;
    public Vector3 size;
}

public class AreaControl : MonoBehaviour
{
    [SerializeField] private Transform enemyTrans;
    [SerializeField] private Vector3 cubeSize;
    private Rectangle _rectangle;
    private List<CubeFacts> _cubeFacts = new ();
    private List<ExtraCubes> _extraCubesList = new();
    [SerializeField] private float delayUpdate;
    private float _timeSinceUpdate;

    private Vector3 _min;
    private Vector3 _max;

    private bool _checkCollider;

    private bool _searchFinished;
    
   // public event Action<>

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
                var next = transform.position + new Vector3(j * cubeSize.x,0,i * cubeSize.z);
                _cubeFacts.Add(new CubeFacts()
                {
                    location = next,
                    color = Color.red,
                    coordinate = new Vector2(j, i)
                });
               
            }
        }
        
        var xArray = _cubeFacts.OrderBy(x => x.location.x).ToArray();
        var zArray = _cubeFacts.OrderBy(x => x.location.z).ToArray();

        _max = Vector3.Max(xArray[^1].location, zArray[^1].location);
        _min = Vector3.Min(xArray[0].location, zArray[0].location);
        
    }

    public void AddCube(Vector3 location, Vector3 size, Color color)
    {
        _extraCubesList.Add(new ExtraCubes()
        {
            location = location,
            size = size,
            color = color
        });
    }
    
    private IEnumerator DelayStopCheck()
    {
        yield return new WaitUntil(() => _searchFinished);
        _checkCollider = false;
    }

    private void TraceTest(Vector2 coordinate)
    {
        var area = _cubeFacts.SingleOrDefault(x => x.coordinate == coordinate);
        if (area == default) return;

        var hits =  Physics.BoxCastAll(area.location, new Vector3(cubeSize.x / 2, 6, cubeSize.z / 2), Vector3.up);
        var tileSize = new Vector3(cubeSize.x, 3, cubeSize.z);
        var tileMin = new Vector3(area.location.x - cubeSize.x / 2, area.location.y, area.location.z - cubeSize.z / 2);
        var tileMax = new Vector3(area.location.x + cubeSize.x / 2, area.location.y + 3, area.location.z + cubeSize.z  / 2);
        var color = new Color(1, 1, 1);
        
        foreach (var h in hits)
        {
            var layer = h.collider.transform.gameObject.layer;
            var objectSize = h.collider.bounds.size;

            if (layer == 9)
            {
                color = Color.black;
                if (objectSize.x > tileSize.x)
                {
                    
                }
            }
            else
            {
                color = Color.yellow;
            }
            
            _extraCubesList.Add(new ExtraCubes()
            {
                
                
                color = color,
                location = h.collider.bounds.center,
                size = objectSize
                
            });
        }
    }
    
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (!_checkCollider) return;
        
        
        _searchFinished = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeSinceUpdate > delayUpdate)
        {
            _timeSinceUpdate -= delayUpdate;
            UpdateColors();
            TraceTest(new Vector2(0,0));
        }
        
        _timeSinceUpdate += Time.deltaTime;
    }

    private void UpdateColors()
    {
        var playerPos = enemyTrans.position;
        
        foreach (var c in _cubeFacts)
        {
            var location = c.location;
            var min = location - new Vector3(cubeSize.x  / 2, 0, cubeSize.z  / 2);
            var max = location + new Vector3(cubeSize.x  / 2, 0, cubeSize.z  / 2);

            if (playerPos.x > min.x && playerPos.z > min.z && playerPos.x < max.x && playerPos.x < max.z)
            {
                c.color = Color.green;
            }
        }
        
       // Debug.Log($"first index: {maxX}, /n  last index: {minX}");
    }
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                var next = transform.position + new Vector3(i * cubeSize.x ,0,j * cubeSize.z);
                Gizmos.DrawWireCube(next, new Vector3(cubeSize.x ,3,cubeSize.z ));
                
            }
        }

        if (!Application.isPlaying) return;
        
        foreach (var c in _cubeFacts)
        {
            Gizmos.color = c.color;
            Gizmos.DrawWireCube(c.location, new Vector3(cubeSize.x ,3,cubeSize.z));
        }

        foreach (var c in _extraCubesList)
        {
            Gizmos.color = c.color;
            Gizmos.DrawWireCube(c.location, c.size);
        }
    }
}
