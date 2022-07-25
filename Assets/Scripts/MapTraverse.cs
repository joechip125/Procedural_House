using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellStatus
{
    Open, Blocked, BlockedAndCloser, OpenAndCloser, OpenAndFurther, OpenAndSame, AlreadyVisited, Null
}

[Serializable]
public class TraverseCell
{
    public float moveValue;
    public float DistanceFromGoal;
    public float TotalMoveValue;
    public CellStatus Status;
    public Vector3 location;

}


public class MapTraverse : MonoBehaviour
{
    private List<TraverseCell> closedList = new ();
    private List<TraverseCell> openList = new ();
    private Vector3 destination;
    private List<TraverseCell> tempList = new();

    public void TraceForWalls()
    {
        tempList.Clear();
        var pos = transform.position + new Vector3(0,1,0);
        for (var i = 0; i < 8; i++)
        {
            var cos =MathF.Cos(MathF.PI / 180 * (i * 45));
            var sin = MathF.Sin(MathF.PI / 180 * (i * 45));
            var end = pos + new Vector3(1 * cos, 0, 1 * sin);
            var ray = new Ray(pos, end);

            tempList.Add(new TraverseCell()
            {
                location = pos + new Vector3(1 * cos, 0, 1 * sin ),
                DistanceFromGoal = Vector3.Distance(end, destination)
            });
            
            if ( Physics.Raycast(ray, out var hit, 2 ))
            {
                Debug.DrawLine(pos, pos + new Vector3(1 * cos, 0, 1 * sin ), Color.red, 99);
            }
            else
            {
                Debug.DrawLine(pos, pos + new Vector3(1 * cos, 0, 1 * sin ), Color.green, 99);
            }
        }
    
    }

    void Start()
    {
        TraceForWalls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
