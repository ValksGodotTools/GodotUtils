namespace GodotUtils;

using TransType = Tween.TransitionType;
using EaseType  = Tween.EaseType;

/*
 * Create a path from a set of points with options to add curvature and
 * animate the attached sprite.
 */
public partial class GPath : Path2D
{
    public bool Rotates 
    { 
        get => PathFollow.Rotates; 
        set => PathFollow.Rotates = value; 
    }

    private PathFollow2D PathFollow  { get; }
    private Vector2[]    Points      { get; }
    private Sprite2D     Sprite      { get; set; }

    private Tween        Tween       { get; set; }
    private float[]      TweenValues { get; set; }
    private int          TweenIndex  { get; set; }
    private TransType    TransType   { get; } = TransType.Sine;
    private EaseType     EaseType    { get; } = EaseType.Out;
    private double       AnimSpeed   { get; }
    private Color        Color       { get; }
    private float        Width       { get; }
    private int          Dashes      { get; }

    public GPath(Vector2[] points, Color color, int width = 5, int dashes = 0, double animSpeed = 1)
    {
        Points = points;
        Curve = new Curve2D();
        PathFollow = new PathFollow2D { Rotates = false };
        AddChild(PathFollow);

        Color = color;
        Width = width;
        Dashes = dashes;
        AnimSpeed = animSpeed;

        // Add points to the path
        for (int i = 0; i < points.Length; i++)
            Curve.AddPoint(points[i]);

        CalculateTweenValues();
    }

    public override void _Draw()
    {
        var points = Curve.GetBakedPoints();

        for (int i = 0; i < points.Length - 1; i += (Dashes + 1))
        {
            var A = points[i];
            var B = points[i + 1];

            DrawLine(A, B, Color, Width, true);
        }
    }

    public void AnimateTo(int targetIndex)
    {
        if (targetIndex > TweenIndex)
            AnimateForwards(targetIndex - TweenIndex);
        else
            AnimateBackwards(TweenIndex - targetIndex);
    }

    public void AnimateForwards(int step = 1)
    {
        TweenIndex = Mathf.Min(TweenIndex + step, TweenValues.Count() - 1);
        Animate(true);
    }

    public void AnimateBackwards(int step = 1)
    {
        TweenIndex = Mathf.Max(TweenIndex - step, 0);
        Animate(false);
    }

    public void AddSprite(Sprite2D sprite)
    {
        Sprite = sprite;
        PathFollow.AddChild(sprite);
    }

    /// <summary>
    /// Add curves to the path. The curve distance is how far each curve is pushed
    /// out.
    /// </summary>
    public void AddCurves(int curveSize = 50, int curveDistance = 50)
    {
        // Add aditional points to make each line be curved
        var invert = 1;

        for (int i = 0; i < Points.Length - 1; i++)
        {
            var A = Points[i];
            var B = Points[i + 1];

            var center = (A + B) / 2;
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

            var index = 1 + i * 2;

            // Insert the curved point at the index in the curve
            Curve.AddPoint(newPos,
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
            TweenValues[i] = Curve.GetClosestOffset(Points[i]);
    }

    private void Animate(bool forwards)
    {
        Tween?.Kill();
        Tween = PathFollow.CreateTween();
        Tween.TweenProperty(PathFollow, "progress", TweenValues[TweenIndex], 
            CalculateDuration(forwards)).SetTrans(TransType).SetEase(EaseType);
    }

    private double CalculateDuration(bool forwards)
    {
        var targetPoint = Curve.SampleBaked(TweenValues[TweenIndex], true);

        var remainingProgress = Curve.GetClosestOffset(targetPoint);
        var currentProgress = PathFollow.Progress;

        var duration = ((forwards ?
            (remainingProgress - currentProgress) :
            (currentProgress - remainingProgress)) / 150) / AnimSpeed;

        return duration;
    }
}
