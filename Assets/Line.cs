using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Perpendicular is the line from the current point to previous point
public struct Line{
    const float VertLineGrad = 1e5f;
    float Gradient;
    float YIntercept;
    Vector2 PointOnLine1;
    Vector2 PointOnLine2;
    float GradientPerpendicular;
    
    bool ApproachSide;

    public Line(Vector2 PointOnLine, Vector2 PointPerpendicularToLine)
    {
        float dx = PointOnLine.x - PointPerpendicularToLine.x;
        float dy = PointOnLine.y - PointPerpendicularToLine.y;
        
        if (dx == 0)
            GradientPerpendicular = VertLineGrad;
        else
            GradientPerpendicular = dy/dx;
        
        if(GradientPerpendicular == 0)
            Gradient = VertLineGrad;
        else
            Gradient = -1/GradientPerpendicular;
        
        YIntercept = PointOnLine.y - Gradient*PointOnLine.x;

        PointOnLine1 = PointOnLine;
        PointOnLine2 = PointOnLine + new Vector2(1, Gradient);

        ApproachSide = false;
        ApproachSide = GetSide(PointPerpendicularToLine);
    }
    //Getw Which side (0 or 1) of the line the Point P is on. The line is defined by two points PointOnLine1 and PointOnLine2
    bool GetSide(Vector2 P)
    {
        return (P.x - PointOnLine1.x) * (PointOnLine2.y-PointOnLine1.y) > (P.y - PointOnLine1.y) * (PointOnLine2.x - PointOnLine1.x);
    }

    //Has Point P crossed the Line
    public bool HasPointCrossedLine(Vector2 P)
    {
        return GetSide(P) != ApproachSide;
    }

    public void DrawWithGizmos(float Length)
    {
        Vector3 LineDir = new Vector3(1, 0, Gradient).normalized;
        Vector3 LineCenter = new Vector3(PointOnLine1.x, 0, PointOnLine1.y) + Vector3.up;
        Gizmos.DrawLine(LineCenter-LineDir * Length/2f, LineCenter+LineDir*Length/2f );
    }
}
