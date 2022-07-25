using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum CellStatus
{
    BlockedAndFurther, BlockedAndCloser,BlockedAndSame, OpenAndCloser, OpenAndFurther, OpenAndSame, AlreadyVisited, Null
}

[Serializable]
public class TraverseCell
{
    public float moveValue;
    public float DistanceFromGoal;
    public float TotalMoveValue;
    public CellStatus Status;
    public Vector3 Position;

}


public class MapTraverse : MonoBehaviour
{
    private List<TraverseCell> closedList = new ();
    private List<TraverseCell> openList = new ();
    public Vector3 destination;
    private List<TraverseCell> tempList = new();
    private TraverseCell currentTraverseCell;
    [SerializeField] private GameObject spawnTest;
    

    public void TraceForWalls(Vector3 currentPos)
    {
        tempList.Clear();
        var pos = currentPos + new Vector3(0,1,0);
        var currentDist = Vector3.Distance(pos, destination);
        for (var i = 0; i < 8; i++)
        {
            var cos =MathF.Cos(MathF.PI / 180 * (i * 45));
            var sin = MathF.Sin(MathF.PI / 180 * (i * 45));
            var end = pos + new Vector3(1 * cos, 0, 1 * sin);
            var ray = new Ray(pos, new Vector3(1 * cos, 0, 1 * sin));
            var hasHit = false;

            tempList.Add(new TraverseCell()
            {
                Position = end - new Vector3(0,1,0),
                DistanceFromGoal = Vector3.Distance(end, destination)
            });
            
            if ( Physics.Raycast(ray, out var hit, 1))
            {
                Debug.DrawLine(pos, pos + new Vector3(1 * cos, 0, 1 * sin ), Color.red, 99);
                hasHit = true;
            }
            else
            {
                Debug.DrawLine(pos, pos + new Vector3(1 * cos, 0, 1 * sin ), Color.green, 99);
                hasHit = false;
            }
            
            if (tempList[^1].DistanceFromGoal < currentDist)
            {
                tempList[^1].Status = hasHit ? CellStatus.BlockedAndCloser : CellStatus.OpenAndCloser;
            }
            else if (tempList[^1].DistanceFromGoal > currentDist)
            {
                tempList[^1].Status = hasHit ? CellStatus.BlockedAndFurther : CellStatus.OpenAndFurther;
            }
            else if ((int)tempList[^1].DistanceFromGoal == (int)currentDist)
            {
                tempList[^1].Status = hasHit ? CellStatus.BlockedAndSame : CellStatus.OpenAndSame;
            }
            
        }
    
    }

    private void Start()
    {
        StartSearch();
    }

    private void StartSearch()
    {
        openList.Clear();
        closedList.Clear();
        currentTraverseCell = new TraverseCell()
        {
            Position = transform.position,
            DistanceFromGoal = Vector3.Distance(transform.position, destination)
        };
        
        openList.Add(currentTraverseCell);
        
        Search();
    }
    
    private void Search()
    {
        var count = 0;

        while(count < 76)
        {
            if (currentTraverseCell.DistanceFromGoal < 2)
            {
                Debug.Log("Destination reached");
                break;
            }
            
            TraceForWalls(currentTraverseCell.Position);
            
            if (tempList.Where(x => x.Status == CellStatus.OpenAndCloser).ToList().Count < 1)
            {
                if (tempList.Where(x => x.Status == CellStatus.BlockedAndCloser).ToList().Count > 1)
                {
                    tempList = tempList.Where(x => x.Status == CellStatus.OpenAndFurther)
                        .Select(x =>
                        {
                            x.moveValue -= 100;
                            return x;
                        }).ToList();
                }
            }
        
            foreach (var t in tempList)
            {
                bool addToOpen = !(t.Status is  CellStatus.BlockedAndCloser or CellStatus.AlreadyVisited or CellStatus.BlockedAndFurther);
                var distanceFromGoal = t.DistanceFromGoal;
                var moveValue = Vector3.Distance(currentTraverseCell.Position, t.Position);
                
                var totalDistanceTraveled = moveValue + distanceFromGoal;
                
                if (!addToOpen) continue;
           
                openList.Add(t);
                openList[^1].TotalMoveValue = totalDistanceTraveled;
            }

            openList = openList
                .OrderBy(x => x.DistanceFromGoal).ToList();
            

            closedList.Add(openList[0]);
            currentTraverseCell = openList[0];
           
            count++;
        }

    }
    
}
