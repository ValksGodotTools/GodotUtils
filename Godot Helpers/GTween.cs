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

    public PropertyTweener AnimateColor(Color color, double duration, bool parallel = false)
    {
        if (node is ColorRect)
            return Animate("color", color, duration, parallel);
        else
            return Animate("self_modulate", color, duration, parallel);
    }

    public PropertyTweener Animate(NodePath prop, Variant finalValue, double duration, bool parallel = false) =>
        parallel ? 
            tween.Parallel().TweenProperty(node, prop, finalValue, duration) :
            tween.TweenProperty(node, prop, finalValue, duration);

    public CallbackTweener Callback(Action callback) =>
        tween.TweenCallback(Callable.From(callback));

    public void Stop() => tween.Stop();
    public void Pause() => tween.Pause();
    public void Resume() => tween.Play();
    public void Kill() => tween?.Kill();
}
