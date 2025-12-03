using UnityEngine;

/// <summary>
/// A parameterizable path that the drill guy will take underground, implemented as a cubic bezier
/// Admittedly I only made an extra class so the drill guy wasn't cluttered - andrew
/// </summary>
public class DrillPath : MonoBehaviour
{
    // The control points for this cubic bezier (must be 4 points)
    private Vector2[] controlPoints;

    /// <summary>
    /// Constructor for this spline
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    public DrillPath(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        controlPoints = new Vector2[4];
        controlPoints[0] = p0;
        controlPoints[1] = p1;
        controlPoints[2] = p2;
        controlPoints[3] = p3;
    }

    /// <summary>
    /// t takes values 0-1, describes how far along we are along this path
    /// which is simply applying the bezier curve formula.
    /// </summary>
    /// <returns>The position along this curve(drill path)</returns>
    public Vector2 GetPositionForT2D(float t)
    {
        Vector2 p0 = controlPoints[0];
        Vector2 p1 = controlPoints[1];
        Vector2 p2 = controlPoints[2];
        Vector2 p3 = controlPoints[3];
        return Mathf.Pow(1 - t, 3) * p0 + 
                3 * Mathf.Pow(1 - t, 2) * t * p1 +
                3 * (1 - t) * Mathf.Pow(t, 2) * p2 + 
                Mathf.Pow(t, 3) * p3;
    }
}