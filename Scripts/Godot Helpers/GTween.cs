namespace GodotUtils;

public class GTween
{
    private Tween tween;
    private Node node;

    public GTween(Node node) => this.node = node;

    public void Create()
    {
        Kill();
        tween = node.CreateTween();
    }

    public PropertyTweener Animate(NodePath prop, Variant finalValue, double duration, bool parallel = false)
    {
        if (parallel)
            return tween.Parallel().TweenProperty(node, prop, finalValue, duration);
        else
            return tween.TweenProperty(node, prop, finalValue, duration);
    }

    public void Callback(Action callback) =>
        tween.TweenCallback(Callable.From(callback));

    public void Kill() => tween?.Kill();
}
