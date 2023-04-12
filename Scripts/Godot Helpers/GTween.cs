namespace GodotUtils;

public class GTween
{
    private Tween Tween { get; set; }
    private Node  Node  { get; }

    public GTween(Node node) => Node = node;

    public void Create()
    {
        Kill();
        Tween = Node.CreateTween();
    }

    public PropertyTweener Animate(NodePath prop, Variant finalValue, double duration, bool parallel = false)
    {
        if (parallel)
            return Tween.Parallel().TweenProperty(Node, prop, finalValue, duration);
        else
            return Tween.TweenProperty(Node, prop, finalValue, duration);
    }

    public void Callback(Action callback) =>
        Tween.TweenCallback(Callable.From(callback));

    public void Kill() => Tween?.Kill();
}
