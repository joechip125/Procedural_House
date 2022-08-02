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

[Flags]
public enum ScanChoices
{
    None = 0,
    Walls = 1,
    OtherEnemies = 2,
    Player = 4,
    All = 1 + 2 + 4,
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

    private List<List<CubeFacts>> _gridList = new();

    private Vector3 _min;
    private Vector3 _max;

    private bool _checkCollider;

    private bool _searchFinished;
    
   // public event Action<>

    private void Awake()
    {
       
    }

    void Start()
    {
        SetNewList();
    }

    public void GetCubeInfos(Vector2 index)
    {
        
    }

    private void SetNewList()
    {
        _gridList = new List<List<CubeFacts>>((int)numberCubes.y);

        for (var i = 0; i < numberCubes.y; i++)
        {
            var newList = new List<CubeFacts>((int)numberCubes.x);
            for (var j = 0; j < numberCubes.x; j++)
            {
                var next = transform.position + new Vector3(j * cubeSize.x,0,i * cubeSize.z);
                var minMax = GetMinMax(next, cubeSize);
                newList.Add( new CubeFacts()
                {
                    color = Color.red,
                    location = next,
                    min = minMax.Item1,
                    max = minMax.Item2,
                });
            }
            _gridList.Add(newList);
        }
 
        _max = _gridList[^1][^1].location + cubeSize / 2;
        _min = _gridList[0][0].location - cubeSize / 2;
    }
    
    private void InitiateList()
    {
        for (var i = 0; i < numberCubes.y; i++)
        {
            for (var j = 0; j < numberCubes.x; j++)
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

    private Vector2 GetIndexFromPoint(Vector3 thePoint)
    {
        var mag = thePoint - _min;
        var indexX = (mag.x - (mag.x % cubeSize.x)) / cubeSize.x;
        var indexZ = (mag.z - (mag.z % cubeSize.z)) / cubeSize.z;

        return new Vector2(indexZ, indexX);
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

    private void GatherCubes(Vector2 index, float extent, Transform characterTrans)
    {
        var charPos = characterTrans.position;
        var firstIndex = GetIndexFromPoint(charPos);
        var forward = characterTrans.forward;
        var right = Vector3.Cross(enemyTrans.forward, -enemyTrans.up);
        Debug.DrawLine(charPos, charPos + forward * extent, Color.green, 1);
        var forwardRotatePlus =
            Quaternion.Euler(0, 50, 0) * forward;
        var forwardRotateMinus =
            Quaternion.Euler(0, -50, 0) * forward;
        
        Debug.DrawLine(charPos, charPos+ forwardRotatePlus * extent, Color.green, 1);
        Debug.DrawLine(charPos, charPos+ forwardRotateMinus * extent, Color.green, 1);
        var indices = new List<Vector2> {firstIndex};
        var offCenterMax = 50;
        var degreeMod = 1.0f;
        degreeMod = Mathf.Clamp(degreeMod, 0f, 1f);
        
        for (var i = 0; i < 4; i++)
        {
            var anIndex = GetIndexFromPoint(charPos + forward * (i * 5));
            var anIndex2 = GetIndexFromPoint(charPos + 
                                             (Quaternion.Euler(0, offCenterMax * degreeMod, 0) * forward) * (i * 5));
            var anIndex3 = GetIndexFromPoint(charPos + 
                                             (Quaternion.Euler(0, offCenterMax * -degreeMod, 0) * forward) * (i * 5));
            
            if(indices.Contains(anIndex)) continue;
            indices.Add(anIndex);
            if(indices.Contains(anIndex2)) continue;
            indices.Add(anIndex2);
            if(indices.Contains(anIndex3)) continue;
            indices.Add(anIndex3);
        }

        foreach (var i in indices)
        {
            ScanCubeAtIndex(i, ScanChoices.All);
        }
        
        ChangeColorsAtIndices(indices, Color.yellow);
    }

    private void ChangeColorsAtIndices(List<Vector2> indices, Color newColor)
    {
        for (int i = 0; i < numberCubes.y; i++)
        {
            for (int j = 0; j < numberCubes.x; j++)
            {
                if (indices.Contains(new Vector2(i, j)))
                {
                    _gridList[i][j].color = newColor;
                }
                else
                {
                    _gridList[i][j].color = Color.red;
                }
            }    
        }
    }

    private void ScanCubeAtIndex(Vector2 index, ScanChoices choices)
    {
        var cube = _gridList[(int) index.y][(int) index.x];
        var hits =  Physics.BoxCastAll(cube.location, 
            new Vector3(cubeSize.x / 2, 6, cubeSize.z / 2), Vector3.up);
        //Physics.BoxCastNonAlloc();
        foreach (var h in hits)
        {
            if (choices.HasFlag(ScanChoices.Walls))
            {
                
            }
            
            if (choices.HasFlag(ScanChoices.OtherEnemies))
            {
            
            }
            
            if (choices.HasFlag(ScanChoices.Player))
            {
            
            }
        }
        
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
            //UpdateColors();
            GatherCubes(new Vector2(0,2),16, enemyTrans);
        }
        
        _timeSinceUpdate += Time.deltaTime;
    }

    private void UpdateColors()
    {
        var enemyPos = enemyTrans.position;
        
        foreach (var c in _cubeFacts)
        {
            if (IsPointInSquare(enemyPos, c.location, cubeSize))
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
        
        foreach (var c in _gridList)
        {
            foreach (var c2 in c)
            {
                Gizmos.color = c2.color;
                Gizmos.DrawWireCube(c2.location, new Vector3(cubeSize.x ,3,cubeSize.z));
            }
        }

        foreach (var c in _extraCubesList)
        {
            Gizmos.color = c.color;
            Gizmos.DrawWireCube(c.location, c.size);
        }
    }
#endif
}
