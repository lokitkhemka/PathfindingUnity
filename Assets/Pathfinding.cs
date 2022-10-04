using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager RequestManager;
    Grid grid;
    void Awake()
    {
        RequestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }


    public void StartFindPath(Vector3 StartPos, Vector3 EndPos)
    {
        StartCoroutine(FindPath(StartPos, EndPos));
    }

    IEnumerator FindPath(Vector3 StartPos, Vector3 EndPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] WayPoints = new Vector3[0];
        bool PathSuccess = false;

        Node StartNode = grid.NodeFromWorldPoint(StartPos);
        Node EndNode = grid.NodeFromWorldPoint(EndPos);


        if(StartNode.Walkable && EndNode.Walkable){
            Heap<Node> OpenSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> ClosedSet = new HashSet<Node>();
            OpenSet.Add(StartNode);

            while(OpenSet.Count>0)
            {
                Node CurrentNode = OpenSet.RemoveFirst();
                


                ClosedSet.Add(CurrentNode);
                //Found the path
                if (CurrentNode == EndNode)
                {
                    sw.Stop();
                    print("Path Found: " + sw.ElapsedMilliseconds + " ms");
                    PathSuccess=true;
                    
                    break;
                }

                foreach (Node neighbor in grid.GetCellNeighbours(CurrentNode))
                {
                    if(!neighbor.Walkable || ClosedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int NewMovementCosttoNeighbor = CurrentNode.GCost + GetDistance(CurrentNode, neighbor) + neighbor.MovementPenalty;
                    if(NewMovementCosttoNeighbor < neighbor.GCost || !(OpenSet.Contains(neighbor)))
                    {
                        neighbor.GCost = NewMovementCosttoNeighbor;
                        neighbor.HCost = GetDistance(neighbor, EndNode);
                        neighbor.Parent = CurrentNode;

                        if(!OpenSet.Contains(neighbor))
                        {
                            OpenSet.Add(neighbor);
                        }
                        else{
                            OpenSet.UpdateItem(neighbor);
                        }
                    }
                }

            }
        }
        yield return null;
        if(PathSuccess)
        {
            WayPoints = RetracePath(StartNode, EndNode);
        }
        RequestManager.FinishedProcessingPath(WayPoints, PathSuccess);
        
    }

    Vector3[] RetracePath(Node StartNode, Node EndNode)
    {
        List<Node> Path = new List<Node>();
        Node CurrentNode = EndNode;

        while(CurrentNode != StartNode)
        {
            Path.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }
        Vector3[] Waypoints = SimplifyPath(Path);
        Array.Reverse(Waypoints);
        return Waypoints;

    }

    Vector3[] SimplifyPath(List<Node> Path)
    {
        List<Vector3> Waypoints = new List<Vector3>();
        Vector2 DirectionOld = Vector2.zero;

        for(int i = 1; i<Path.Count; i++)
        {
            Vector2 DirectionNew = new Vector2(Path[i-1].GridX - Path[i].GridX,Path[i-1].GridY - Path[i].GridY);
            if(DirectionNew != DirectionOld)
            {
                Waypoints.Add(Path[i].WorldPosition);
            }

            DirectionOld = DirectionNew;
        }

        return Waypoints.ToArray();
    }

    int GetDistance(Node NodeA, Node NodeB)
    {
        int DistX =  Mathf.Abs(NodeA.GridX - NodeB.GridX);
        int DistY =  Mathf.Abs(NodeA.GridY - NodeB.GridY);

        if(DistX > DistY)
        {
            return 14*DistY + 10*(DistX-DistY);
        }
        return 14*DistX + 10*(DistY-DistX);
    }
}
