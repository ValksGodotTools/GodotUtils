namespace GodotUtils;

using TransType = Tween.TransitionType;
using EaseType  = Tween.EaseType;

public class GPath
{
    private Path2D       Path        { get; }
    private PathFollow2D PathFollow  { get; }
    private Vector2[]    Points      { get; }

    private Tween        Tween       { get; set; }
    private float[]      TweenValues { get; set; }
    private int          TweenIndex  { get; set; }
    private TransType    TransType   { get; } = TransType.Sine;
    private EaseType     EaseType    { get; } = EaseType.Out;
    private double       Duration    { get; } = 1.5;

    public GPath(Node parent, Vector2[] points)
    {
        Points = points;
        Path = new Path2D { Curve = new Curve2D() };
        PathFollow = new PathFollow2D { Rotates = false };
        Path.AddChild(PathFollow);
        parent.AddChild(Path);

        // Add points to the path
        for (int i = 0; i < points.Length; i++)
            Path.Curve.AddPoint(points[i]);

        CalculateTweenValues();
    }

    public void AnimateNext()
    {
        TweenIndex = Mathf.Min(TweenIndex + 1, TweenValues.Count() - 1);

        Tween?.Kill();
        Tween = PathFollow.CreateTween();
        Tween.TweenProperty(PathFollow, "progress", TweenValues[TweenIndex], Duration)
            .SetTrans(TransType)
            .SetEase(EaseType);
    }

    public void AnimatePrev()
    {
        TweenIndex = Mathf.Max(TweenIndex - 1, 0);

        Tween?.Kill();
        Tween = PathFollow.CreateTween();
        Tween.TweenProperty(PathFollow, "progress", TweenValues[TweenIndex], Duration)
            .SetTrans(TransType)
            .SetEase(EaseType);
    }

    public void AddSprite(Sprite2D node) => PathFollow.AddChild(node);

    public void AddCurves()
    {
        // Add aditional points to make each line be curved
        var invert = 1;

        for (int i = 0; i < Points.Length - 1; i++)
        {
            var A = Points[i];
            var B = Points[i + 1];

            var center = (A + B) / 2;
            var curveDistance = 50; // How far the curve is pushed out
            var offset = ((B - A).Orthogonal().Normalized() * curveDistance * invert);
            var newPos = center + offset;

            // Switch between sides so curves flow more naturally
            invert *= -1;

            Vector4 v;

            // These values were found through trial and error
            // If you see a simpler pattern than this, please tell me lol
            if (B.Y >= A.Y)
                if (B.X >= A.X)
                    // Next point is under and after first point
                    v = new Vector4(-1, -1, 1, 1);
                else
                    // Next point is under and before first point
                    v = new Vector4(1, -1, -1, 1);
            else
                if (B.X <= A.X)
                // Next point is over and before first point
                v = new Vector4(1, 1, -1, -1);
            else
                // Next point is over and after first point
                v = new Vector4(-1, 1, 1, -1);

            var curveSize = 50; // How big the curve is
            var index = 1 + i * 2;

            // Insert the curved point at the index in the curve
            Path.Curve.AddPoint(newPos,
                new Vector2(v.X, v.Y) * curveSize,
                new Vector2(v.Z, v.W) * curveSize, index);
        }

        // Since new points were added, the tween values need to be re-calulcated
        CalculateTweenValues();
    }

    private void CalculateTweenValues()
    {
        TweenValues = new float[Points.Length];
        for (int i = 0; i < Points.Length; i++)
            TweenValues[i] = Path.Curve.GetClosestOffset(Points[i]);
    }
}
