namespace GodotUtils.Deprecated;

using Godot;

/*
 * No longer being used because Bezier Curves are built into the Godot Game Engine
 * There is no need to re-invent the wheel. I won't delete this class because I did
 * spend an insane amount of time trying to figure this stuff out.
 */
public static class BezierCurve
{
    public static Vector2[] Draw(CanvasItem node, Vector2 pointA, Vector2 pointB, Vector2 curve1, Vector2 curve2, Color color, int width = 5, float resolution = 0.01f)
    {
        var points = GetPoints(pointA, pointB, curve1, curve2, resolution);

        for (int i = 0; i < points.Length - 1; i++)
            node.DrawLine(points[i], points[i + 1], color, width, true);

        return points;
    }

    public static Vector2[] GetPoints(Vector2 pointA, Vector2 pointB, Vector2 curve1, Vector2 curve2, float resolution = 0.01f)
    {
        var p0 = pointA;
        var p1 = pointB + curve1;
        var p2 = pointA + curve2;
        var p3 = pointB;

        var numPoints = (int)(1 / resolution) + 1;
        var points = new Vector2[numPoints];

        var i = 0;
        for (float t = 0; t <= 1.0f; t += resolution)
            points[i++] = GetPoint(t, p0, p1, p2, p3);

        return points;
    }

    // Yoinked from https://www.icode.com/c-function-for-a-bezier-curve/
    static Vector2 GetPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        var cx = 3 * (p1.X - p0.X);
        var cy = 3 * (p1.Y - p0.Y);
        var bx = 3 * (p2.X - p1.X) - cx;
        var by = 3 * (p2.Y - p1.Y) - cy;
        var ax = p3.X - p0.X - cx - bx;
        var ay = p3.Y - p0.Y - cy - by;
        var Cube = t * t * t;
        var Square = t * t;

        var resX = (ax * Cube) + (bx * Square) + (cx * t) + p0.X;
        var resY = (ay * Cube) + (by * Square) + (cy * t) + p0.Y;

        return new Vector2(resX, resY);
    }
}
