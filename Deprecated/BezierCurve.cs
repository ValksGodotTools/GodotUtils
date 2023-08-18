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
        Vector2[] points = GetPoints(pointA, pointB, curve1, curve2, resolution);

        for (int i = 0; i < points.Length - 1; i++)
            node.DrawLine(points[i], points[i + 1], color, width, true);

        return points;
    }

    public static Vector2[] GetPoints(Vector2 pointA, Vector2 pointB, Vector2 curve1, Vector2 curve2, float resolution = 0.01f)
    {
        Vector2 p0 = pointA;
        Vector2 p1 = pointB + curve1;
        Vector2 p2 = pointA + curve2;
        Vector2 p3 = pointB;

        int numPoints = (int)(1 / resolution) + 1;
        Vector2[] points = new Vector2[numPoints];

        int i = 0;
        for (float t = 0; t <= 1.0f; t += resolution)
            points[i++] = GetPoint(t, p0, p1, p2, p3);

        return points;
    }

    // Yoinked from https://www.icode.com/c-function-for-a-bezier-curve/
    static Vector2 GetPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float cx = 3 * (p1.X - p0.X);
        float cy = 3 * (p1.Y - p0.Y);
        float bx = 3 * (p2.X - p1.X) - cx;
        float by = 3 * (p2.Y - p1.Y) - cy;
        float ax = p3.X - p0.X - cx - bx;
        float ay = p3.Y - p0.Y - cy - by;
        float Cube = t * t * t;
        float Square = t * t;
        float resX = (ax * Cube) + (bx * Square) + (cx * t) + p0.X;
        float resY = (ay * Cube) + (by * Square) + (cy * t) + p0.Y;

        return new Vector2(resX, resY);
    }
}
