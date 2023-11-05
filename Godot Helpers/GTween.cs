namespace GodotUtils;

using Godot;
using System;

/// <summary>
/// The created GTween should be defined in _Ready() if it is going to be re-used
/// several times with GTween.Create()
/// </summary>
public class GTween
{
    Tween tween;
    Node node;

    public GTween(Node node)
    {
        this.node = node;
        Kill();
        tween = node.CreateTween();

        // This helps to prevent the camera from lagging behind the players movement
        tween.SetProcessMode(Tween.TweenProcessMode.Physics);
    }

    public void StopLooping() => tween.SetLoops(1);
    public void Loop() => tween.SetLoops();

    public PropertyTweener AnimateColor(Color color, double duration, bool modulateChildren = false, bool parallel = false)
    {
        if (node is ColorRect)
            return Animate("color", color, duration, parallel);
        else
        {
            if (modulateChildren)
            {
                return Animate("modulate", color, duration, parallel);
            }
            else
            {
                return Animate("self_modulate", color, duration, parallel);
            }
        }
    }

    public PropertyTweener Animate(string prop, Variant finalValue, double duration, bool parallel = false) =>
        parallel ?
            tween.Parallel().TweenProperty(node, prop, finalValue, duration) :
            tween.TweenProperty(node, prop, finalValue, duration);

    public CallbackTweener Callback(Action callback, bool parallel = false)
    {
        if (!parallel)
            return tween.TweenCallback(Callable.From(callback));
        else
            return tween.Parallel().TweenCallback(Callable.From(callback));
    }

    public void Delay(double duration) =>
        tween.TweenCallback(Callable.From(() => { /* Empty Action */ })).SetDelay(duration);

    public void Finished(Action callback) => tween.Finished += callback;
    public bool IsRunning() => tween.IsRunning();
    public void Stop() => tween.Stop();
    public void Pause() => tween.Pause();
    public void Resume() => tween.Play();
    public void Kill() => tween?.Kill();
}
