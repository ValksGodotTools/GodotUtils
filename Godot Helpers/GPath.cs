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
        get => pathFollow.Rotates; 
        set => pathFollow.Rotates = value; 
    }

    private PathFollow2D pathFollow;
    private Vector2[] points;
    private Sprite2D sprite;

    private GTween tween;
    private float[] tweenValues;
    private int tweenIndex;
    private TransType transType = TransType.Sine;
    private EaseType easeType = EaseType.Out;
    private double animSpeed;
    private Color color;
    private float width;
    private int dashes;

    public GPath(Vector2[] points, Color color, int width = 5, int dashes = 0, double animSpeed = 1)
    {
        this.points = points;
        Curve = new Curve2D();
        pathFollow = new PathFollow2D { Rotates = false };
        tween = new GTween(pathFollow);
        AddChild(pathFollow);

        this.color = color;
        this.width = width;
        this.dashes = dashes;
        this.animSpeed = animSpeed;

        // Add points to the path
        for (int i = 0; i < points.Length; i++)
            Curve.AddPoint(points[i]);

        CalculateTweenValues();
    }

    public override void _Draw()
    {
        var points = Curve.GetBakedPoints();

        for (int i = 0; i < points.Length - 1; i += (dashes + 1))
        {
            var A = points[i];
            var B = points[i + 1];

            DrawLine(A, B, color, width, true);
        }
    }

    public void SetLevelProgress(int v) => pathFollow.Progress = tweenValues[v - 1];

    public void AnimateTo(int targetIndex)
    {
        if (targetIndex > tweenIndex)
            AnimateForwards(targetIndex - tweenIndex);
        else
            AnimateBackwards(tweenIndex - targetIndex);
    }

    public int AnimateForwards(int step = 1)
    {
        tweenIndex = Mathf.Min(tweenIndex + step, tweenValues.Count() - 1);
        Animate(true);
        return tweenIndex;
    }

    public int AnimateBackwards(int step = 1)
    {
        tweenIndex = Mathf.Max(tweenIndex - step, 0);
        Animate(false);
        return tweenIndex;
    }

    public void AddSprite(Sprite2D sprite)
    {
        this.sprite = sprite;
        pathFollow.AddChild(sprite);
    }

    /// <summary>
    /// Add curves to the path. The curve distance is how far each curve is pushed
    /// out.
    /// </summary>
    public void AddCurves(int curveSize = 50, int curveDistance = 50)
    {
        // Add aditional points to make each line be curved
        var invert = 1;

        for (int i = 0; i < points.Length - 1; i++)
        {
            var A = points[i];
            var B = points[i + 1];

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
        tweenValues = new float[points.Length];
        for (int i = 0; i < points.Length; i++)
            tweenValues[i] = Curve.GetClosestOffset(points[i]);
    }

    private void Animate(bool forwards)
    {
        tween = new(this);
        tween.Animate("progress", tweenValues[tweenIndex], 
            CalculateDuration(forwards)).SetTrans(transType).SetEase(easeType);
    }

    private double CalculateDuration(bool forwards)
    {
        // The remaining distance left to go from the current sprites progress
        var remainingDistance = Mathf.Abs(
            tweenValues[tweenIndex] - pathFollow.Progress);

        var startIndex = 0;

        // Dynamically calculate the start index
        for (int i = 0; i < tweenValues.Length; i++)
            if (pathFollow.Progress <= tweenValues[i])
            {
                startIndex = i;
                break;
            }

        // The number of level icons left to pass
        var levelIconsLeft = Mathf.Max(1, Mathf.Abs(tweenIndex - startIndex));
        
        var duration = remainingDistance / 150 / animSpeed / levelIconsLeft;

        return duration;
    }
}
