using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Static class for Bezier curve calculation
/// </summary>
public class Bezier
{
    public static readonly int BezierIterationsCount = 15;

    public static Vector3 CenterOf(Vector3 start, Vector3 end)
    {
        return start * 0.5f + end * 0.5f;
    }

    public static Vector3 Calculate(Vector3 start, Vector3 middle, Vector3 end, float t)
    {
        return Mathf.Pow(1f - t, 2) * start + 2 * (1f - t) * t * middle + Mathf.Pow(t, 2) * end;
    }
    
    public static Vector3 CalculateH(Vector3 start, Vector3 middle, Vector3 end, float t, float hard = 2f)
    {
        return Vector3.Lerp(Mathf.Pow(1f - t, 2) * start + 2 * (1f - t) * t * middle + Mathf.Pow(t, 2) * end, middle, ((1f - t) * t) * hard);
    }

    public static Vector3 GetPointPosition(Vector3 start, Vector3 end, float t, float multiply)
    {
        Vector3 additionalPoint = Bezier.CenterOf(start, end) - new Vector3(0, multiply * Vector3.Distance(start, end) / 2, 0);

        return Bezier.Calculate(start, additionalPoint, end, t);
    }

    public static Vector3 GetPointPosition(Vector3 start, Vector3 end, Vector3 helper, float t)
    {
        return Bezier.Calculate(start, helper, end, t);
    }
}
