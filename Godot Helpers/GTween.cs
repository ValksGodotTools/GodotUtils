namespace GodotUtils;

using Godot;
using System;

/// <summary>
/// The created GTween should be defined in _Ready() if it is going to be re-used
/// several times
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
    /// Creates a delay followed by a callback only executed after the delay
    /// </summary>
    public static void Delay(Node node, double duration, Action callback)
    {
        GTween tween = new(node);
        tween.Delay(duration);
        tween.Callback(callback);
    }

    public void SetProcessMode(Tween.TweenProcessMode mode) =>
        tween.SetProcessMode(mode);

    /// <summary>
    /// Sets the animation to repeat
    /// </summary>
    public void Loop(int loops = 0) => tween.SetLoops(loops);

    /// <summary>
    /// Sets the property to be animated on
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public void SetAnimatingProp(string animatingProperty) =>
        this.animatingProperty = animatingProperty;

    /// <summary>
    /// Animates the property that was set with SetAnimatingProp(string prop)
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GPropertyTweener AnimateProp(Variant finalValue, double duration, bool parallel = false)
    {
        if (string.IsNullOrWhiteSpace(animatingProperty))
            throw new Exception("No animating property has been set");

        return Animate(animatingProperty, finalValue, duration, parallel);
    }

    /// <summary>
    /// Animates a specified property. All tweens use the Sine transition by default.
    /// 
    /// <code>
    /// tween.Animate(ColorRect.PropertyName.Color, Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GPropertyTweener Animate(string prop, Variant finalValue, double duration, bool parallel = false)
    {
        if (parallel)
            tween = tween.Parallel();

        PropertyTweener tweener = tween
            .TweenProperty(node, prop, finalValue, duration)
            .SetTrans(Tween.TransitionType.Sine);

        return new GPropertyTweener(tweener);
    }     

    /// <summary>
    /// The callback is executed in sync with the animation
    /// </summary>
    public CallbackTweener Callback(Action callback, bool parallel = false)
    {
        if (!parallel)
            return tween.TweenCallback(Callable.From(callback));
        else
            return tween.Parallel().TweenCallback(Callable.From(callback));
    }

    /// <summary>
    /// Delay the animation by a specified duration
    /// </summary>
    public void Delay(double duration) =>
        tween.TweenCallback(Callable.From(() => { /* Empty Action */ })).SetDelay(duration);

    /// <summary>
    /// Executed when the tween has finished
    /// </summary>
    public void Finished(Action callback) => tween.Finished += callback;

    /// <summary>
    /// Checks if the tween is still playing
    /// </summary>
    public bool IsRunning() => tween.IsRunning();

    /// <summary>
    /// If the tween is looping, this can be used to stop it
    /// </summary>
    public void Stop() => tween.Stop();

    /// <summary>
    /// Pause the tween
    /// </summary>
    public void Pause() => tween.Pause();

    /// <summary>
    /// If the tween was paused with Pause(), resume it with Resume()
    /// </summary>
    public void Resume() => tween.Play();

    /// <summary>
    /// Kill the tween
    /// </summary>
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

    public GPropertyTweener Linear()
    {
        tweener.SetTrans(Tween.TransitionType.Linear);
        return this;
    }

    public GPropertyTweener Back()
    {
        tweener.SetTrans(Tween.TransitionType.Back);
        return this;
    }

    public GPropertyTweener Sine()
    {
        tweener.SetTrans(Tween.TransitionType.Sine);
        return this;
    }

    public GPropertyTweener Bounce()
    {
        tweener.SetTrans(Tween.TransitionType.Bounce);
        return this;
    }

    public GPropertyTweener Circ()
    {
        tweener.SetTrans(Tween.TransitionType.Circ);
        return this;
    }

    public GPropertyTweener Cubic()
    {
        tweener.SetTrans(Tween.TransitionType.Cubic);
        return this;
    }

    public GPropertyTweener Elastic()
    {
        tweener.SetTrans(Tween.TransitionType.Elastic);
        return this;
    }

    public GPropertyTweener Expo()
    {
        tweener.SetTrans(Tween.TransitionType.Expo);
        return this;
    }

    public GPropertyTweener Quad()
    {
        tweener.SetTrans(Tween.TransitionType.Quad);
        return this;
    }

    public GPropertyTweener Quart()
    {
        tweener.SetTrans(Tween.TransitionType.Quart);
        return this;
    }

    public GPropertyTweener Quint()
    {
        tweener.SetTrans(Tween.TransitionType.Quint);
        return this;
    }

    public GPropertyTweener Spring()
    {
        tweener.SetTrans(Tween.TransitionType.Spring);
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

    public GPropertyTweener EaseInOut()
    {
        tweener.SetEase(Tween.EaseType.InOut);
        return this;
    }

    public GPropertyTweener EaseOutIn()
    {
        tweener.SetEase(Tween.EaseType.OutIn);
        return this;
    }
}
