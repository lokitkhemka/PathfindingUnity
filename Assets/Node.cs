using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool Walkable;
    public Vector3 WorldPosition;
    public int GridX, GridY;
    public Node Parent;
    public int MovementPenalty;

    public int GCost;
    public int HCost;
    int heapIndex;

    public Node(bool walkable, Vector3 worldPos, int gridX, int gridY, int _Penalty)
    {
        Walkable = walkable;
        WorldPosition = worldPos;
        GridX = gridX;
        GridY = gridY;
        MovementPenalty = _Penalty;
    }

    public int FCost
    {
        get
        {
            return GCost+HCost;
        }
    }

    public int HeapIndex{
        get{
            return heapIndex;
        }
        set{
            heapIndex = value;
        }
    }


    public int CompareTo(Node CompareNode)
    {
        int Compare = FCost.CompareTo(CompareNode.FCost);

        if (Compare == 0)
        {
            Compare = HCost.CompareTo(CompareNode.HCost);
        }

        return -Compare;
    }


}
