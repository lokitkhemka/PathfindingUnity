using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public readonly Vector3[] LookPoints;
    public readonly Line[] TurnBoundaries;

    public readonly int FinishLineIndex;

    public Path(Vector3[] Waypoints, Vector3 StartPos, float TurnDist)
    {
        LookPoints = Waypoints;
        TurnBoundaries = new Line[LookPoints.Length];
        FinishLineIndex = TurnBoundaries.Length -1 ;

        Vector2 PrevPoint = V3ToV2(StartPos);

        for(int i = 0; i<LookPoints.Length; i++)
        {
            Vector2 CurrentPoint = V3ToV2(LookPoints[i]);
            Vector2 DirToCurrentPoint = (CurrentPoint-PrevPoint).normalized;
            Vector2 TurnBoundaryPoint = (i == FinishLineIndex)?CurrentPoint:CurrentPoint - DirToCurrentPoint * TurnDist;
            TurnBoundaries[i] = new Line(TurnBoundaryPoint, PrevPoint - DirToCurrentPoint * TurnDist);
            PrevPoint = TurnBoundaryPoint;
        }


    }

    Vector2 V3ToV2(Vector3 V3)
    {
        return new Vector2(V3.x, V3.z);
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach(Vector3 P in LookPoints)
        {
            Gizmos.DrawCube(P+Vector3.up, Vector3.one);   
        }

        Gizmos.color = Color.white;
        foreach (Line L in TurnBoundaries)
        {
            L.DrawWithGizmos(10);
        }
    }


}
