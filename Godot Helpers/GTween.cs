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
    string animatingProperty;

    public GTween(Node node)
    {
        this.node = node;

        // Ensure the Tween is fresh when re-creating it
        Kill();
        tween = node.CreateTween();

        // This helps to prevent the camera from lagging behind the players movement
        tween.SetProcessMode(Tween.TweenProcessMode.Physics);
    }

    /// <summary>
    /// Creates a looping tween that will stop and execute a callback when a condition is met.
    /// </summary>
    public static void Loop(Node node, double duration, Func<bool> condition, Action callback)
    {
        GTween tween = new(node);
        tween.Delay(duration);
        tween.Loop();
        tween.Callback(() =>
        {
            if (condition())
            {
                tween.Stop();
                callback();
            }
        });
    }

    /// <summary>
    /// Creates a delay followed by a callback only executed after the delay. The
    /// tween is attached to a node so when this node gets QueueFree'd the tween
    /// will get QueueFree'd as well.
    /// </summary>
    public static void Delay(Node node, double duration, Action callback)
    {
        GTween tween = new(node);
        tween.Delay(duration);
        tween.Callback(callback);
    }

    public void SetProcessMode(Tween.TweenProcessMode mode) =>
        tween.SetProcessMode(mode);

    public void StopLooping() => tween.SetLoops(1);
    public void Loop(int loops = 0) => tween.SetLoops(loops);

    public void SetAnimatingProp(string animatingProperty) =>
        this.animatingProperty = animatingProperty;

    public GPropertyTweener AnimateProp(Variant finalValue, double duration, bool parallel = false)
    {
        if (string.IsNullOrWhiteSpace(animatingProperty))
            throw new Exception("No animating property has been set");

        return Animate(animatingProperty, finalValue, duration, parallel);
    }

    public GPropertyTweener Animate(string prop, Variant finalValue, double duration, bool parallel = false)
    {
        if (parallel)
            tween = tween.Parallel();

        PropertyTweener tweener = tween.TweenProperty(node, prop, finalValue, duration);

        return new GPropertyTweener(tweener);
    }     

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

public class GPropertyTweener
{
    PropertyTweener tweener;

    public GPropertyTweener(PropertyTweener tweener)
    {
        this.tweener = tweener;
    }

    public GPropertyTweener SetTrans(Tween.TransitionType transType)
    {
        tweener.SetTrans(transType);
        return this;
    }

    public GPropertyTweener SetEase(Tween.EaseType easeType)
    {
        tweener.SetEase(easeType);
        return this;
    }

    public GPropertyTweener Sine()
    {
        tweener.SetTrans(Tween.TransitionType.Sine);
        return this;
    }

    public GPropertyTweener EaseIn()
    {
        tweener.SetEase(Tween.EaseType.In);
        return this;
    }

    public GPropertyTweener EaseOut()
    {
        tweener.SetEase(Tween.EaseType.Out);
        return this;
    }
}
