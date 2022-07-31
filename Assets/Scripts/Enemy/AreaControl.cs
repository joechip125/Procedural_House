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

public enum CompassPoints
{
    E,
    NE,
    N,
    NW,
    W,
    SW,
    S,
    SE,
    Center
}


[Serializable]
public class CubeFacts
{
    public Vector3 location;
    public Color color;
    public Vector3 min;
    public Vector3 max;
}

[Serializable]
public class ExtraCubes
{
    public Vector3 location;
    public Color color;
    public Vector3 size;
}

[Serializable]
public class PointFacts
{
    public Vector3 location;
    public bool pointInSearchArea;
    public Vector2 coordinate;
}

[Serializable]
public class GridPaths
{
    public List<Vector3> travelPoints = new();
}

public class AreaControl : MonoBehaviour
{
    [Header("Input values")]
    [SerializeField] private Transform enemyTrans;
    [SerializeField] private Vector3 cubeSize;
    [SerializeField] private Vector2 numberCubes;
    [Range(0, 1),SerializeField] private float delayUpdate;

    private List<CubeFacts> _cubeFacts = new ();
    private List<ExtraCubes> _extraCubesList = new();
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
        
    }

    private void InitiateList()
    {
        for (var i = 0; i < 10; i++)
        {
            for (var j = 0; j < 10; j++)
            {
                var next = transform.position + new Vector3(j * cubeSize.x,0,i * cubeSize.z);
                var minMax = GetMinMax(next, cubeSize);
                _cubeFacts.Add(new CubeFacts()
                {
                    location = next,
                    color = Color.red,
                    min = minMax.Item1,
                    max = minMax.Item2,
                });
            }
        }
        
        var xArray = _cubeFacts.OrderBy(x => x.location.x).ToArray();
        var zArray = _cubeFacts.OrderBy(x => x.location.z).ToArray();

        _max = Vector3.Max(xArray[^1].location, zArray[^1].location);
        _min = Vector3.Min(xArray[0].location, zArray[0].location);
        
    }

    private void GetCubeAtPoint(Vector3 thePoint)
    {
        if (!IsPointInMinMaxOfSquare(thePoint, _min, _max)) return;
        var currMin = _min;
        var currMax = _min;
        var size = new Vector3(_max.x - _min.x, _max.y - _min.y, _max.z - _min.z);
        var center = new Vector3(_min.x + size.x / 2, _min.y, _min.z + size.z / 2);
        var minMaxIndexX = new Vector2(0, numberCubes.x);
        var minMaxIndexZ = new Vector2(0, numberCubes.y);

        
       var retVal = _cubeFacts.SingleOrDefault(c => thePoint.x > c.min.x && thePoint.x < c.max.x && 
                              thePoint.z < c.min.z && thePoint.z > c.max.z);

      //  size = size / 2;

        var smallerThanCenterX = thePoint.x < center.x;
        var smallerThanCenterZ = thePoint.z < center.z;
        
        
        if (smallerThanCenterX)
        {
            currMax.x = currMin.x + size.x / 2;
            minMaxIndexX.y = numberCubes.x / 2;
        }
        else 
        {
            currMin.x += size.x / 2;
            minMaxIndexX.x =  numberCubes.x / 2;
        }

        if (smallerThanCenterZ)
        {
            currMax.z = currMin.z + size.z / 2;
            minMaxIndexZ.y = numberCubes.y / 2;
        }
        else
        {
            currMin.z += size.z / 2;
            minMaxIndexX.x =  numberCubes.y / 2;
        }
    }
    private void GatherCubes(Vector3 position)
    {
        
    }

    public CubeFacts GetCubeFromCompass(CompassPoints point, Vector2 currentIndex)
    {
        
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
        var area = _cubeFacts[(int)coordinate.x + (int)coordinate.y];
        if (area == default) return;

        var hits =  Physics.BoxCastAll(area.location, new Vector3(cubeSize.x / 2, 6, cubeSize.z / 2), Vector3.up);
        var tileMinMax = GetMinMax(area.location, cubeSize);
        var color = new Color(1, 1, 1);
        
        foreach (var h in hits)
        {
            var layer = h.collider.transform.gameObject.layer;
            var objectSize = h.collider.bounds.size;

            if (layer == 9)
            {
                color = Color.black;
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

    private void GetPointInBoxAtCompassPoint(Vector2 boxCoordinate, CompassPoints point)
    {
        var center = GetCubeAtCoordinates(boxCoordinate).location;
        
        var minMax =
            GetMinMax(center, cubeSize);
    }

    private void DoLineTrace(Vector2 boxCoordinate, Vector3 direction)
    {
        var cube = GetCubeAtCoordinates(boxCoordinate);

        var tracePoint = cube.location + new Vector3(0, 0.4f, 0);
        var ray = new Ray(tracePoint, direction);
        var distance = 5f;

        Physics.Raycast(ray, out var hit, distance);
        

    }

    private CubeFacts GetCubeAtCoordinates(Vector2 boxCoordinate)
    {
        return _cubeFacts[(int) boxCoordinate.x + (int) boxCoordinate.y];
    }
    
    public Vector2 GetAssetQuadrant(Vector3 sizeOfCube, Vector3 assetLoc, Vector3 cubeCenter)
    {
        var minMax  = GetMinMax(cubeCenter, sizeOfCube);
        var outVec = new Vector2();

        if (assetLoc.x > cubeCenter.x)
        {
            outVec.x = 1f;
        }
        
        if (assetLoc.z > cubeCenter.z)
        {
            outVec.y = 1f;
        }
        
        return outVec;
    }

    private bool DoesItBlock()
    {
       // var point1 = new Vector3()

       return false;
    }
    
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
            if (IsPointInSquare(playerPos, c.location, cubeSize))
            {
                c.color = Color.green;
            }
            else
            {
                c.color = Color.red;
            }
        }
    }

    private Tuple<Vector3, Vector3> GetMinMax(Vector3 location, Vector3 size)
    {
        var min = location - new Vector3(size.x  / 2, 0, size.z  / 2);
        var max = location + new Vector3(size.x  / 2, size.y, size.z  / 2);

        return new Tuple<Vector3, Vector3>(min, max);
    }

    private bool IsPointInSquare(Vector3 point, Vector3 squareLocation, Vector3 squareSize)
    {
        var minAndMax = GetMinMax(squareLocation, squareSize);

        if (point.x < minAndMax.Item1.x || point.x > minAndMax.Item2.x) return false;

        if (point.z < minAndMax.Item1.z || point.z > minAndMax.Item2.z) return false;

        return true;
    }

    private bool IsPointInMinMaxOfSquare(Vector3 point, Vector3 min, Vector3 max)
    {
        if (point.x < min.x || point.x > max.x) return false;
        
        if (point.z < min.z || point.z > max.z) return false;

        return true;
    }
    
    
#if UNITY_EDITOR
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
#endif
}
