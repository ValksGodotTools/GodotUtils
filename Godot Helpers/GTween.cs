namespace GodotUtils;

using Godot;
using System;
using static Godot.Tween;

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
    public static GTween Loop(Node node, double duration, Func<bool> condition, Action callback)
    {
        GTween tween = new(node);
        tween.Delay(duration)
            .Loop()
            .Callback(() =>
            {
                if (condition())
                {
                    tween.Stop();
                    callback();
                }
            });

        return tween;
    }

    /// <summary>
    /// Creates a delay followed by a callback only executed after the delay
    /// </summary>
    public static GTween Delay(Node node, double duration, Action callback)
    {
        GTween tween = new(node);

        tween.Delay(duration)
            .Callback(callback);

        return tween;
    }

    /// <summary>
    /// Animates the property that was set with SetAnimatingProp(string prop)
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GTweenerBuilder AnimateProp(Variant finalValue, double duration)
    {
        if (string.IsNullOrWhiteSpace(animatingProperty))
            throw new Exception("No animating property has been set");

        return Animate(animatingProperty, finalValue, duration);
    }

    /// <summary>
    /// Animates a specified property. All tweens use the Sine transition by default.
    /// 
    /// <code>
    /// tween.Animate(ColorRect.PropertyName.Color, Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GTweenerBuilder Animate(string prop, Variant finalValue, double duration)
    {
        PropertyTweener tweener = tween
            .TweenProperty(node, prop, finalValue, duration)
            .SetTrans(Tween.TransitionType.Sine);

        return new GTweenerBuilder(this, tweener);
    }

    /// <summary>
    /// Sets the property to be animated on
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public GTween SetAnimatingProp(string animatingProperty)
    {
        this.animatingProperty = animatingProperty;
        return this;
    }

    public GTween SetProcessMode(Tween.TweenProcessMode mode)
    {
        tween = tween.SetProcessMode(mode);
        return this;
    }

    /// <summary>
    /// Sets the animation to repeat
    /// </summary>
    public GTween Loop(int loops = 0)
    {
        tween = tween.SetLoops(loops);
        return this;
    }

    /// <summary>
    /// <para>Makes the next <see cref="Godot.Tweener"/> run parallelly to the previous one.</para>
    /// <para><b>Example:</b></para>
    /// <para><code>
    /// GTween tween = new(...);
    /// tween.Animate(...);
    /// tween.Parallel().Animate(...);
    /// tween.Parallel().Animate(...);
    /// </code></para>
    /// <para>All <see cref="Godot.Tweener"/>s in the example will run at the same time.</para>
    /// <para>You can make the <see cref="Godot.Tween"/> parallel by default by using <see cref="Godot.Tween.SetParallel(bool)"/>.</para>
    /// </summary>
    public GTween Parallel()
    {
        tween = tween.Parallel();
        return this;
    }

    /// <summary>
    /// <para>If <paramref name="parallel"/> is <see langword="true"/>, the <see cref="Godot.Tweener"/>s appended after this method will by default run simultaneously, as opposed to sequentially.</para>
    /// <para><code>
    /// tween.SetParallel()
    /// tween.Animate(...)
    /// tween.Animate(...)
    /// </code></para>
    /// </summary>
    public GTween SetParallel(bool parallel = true)
    {
        tween = tween.SetParallel(parallel);
        return this;
    }

    public GTween Callback(Action callback)
    {
        tween.TweenCallback(Callable.From(callback));
        return this;
    }

    public GTween Delay(double duration)
    {
        tween.TweenCallback(Callable.From(() => { /* Empty Action */ })).SetDelay(duration);
        return this;
    }

    /// <summary>
    /// Executed when the tween has finished
    /// </summary>
    public GTween Finished(Action callback)
    {
        tween.Finished += callback;
        return this;
    }

    /// <summary>
    /// If the tween is looping, this can be used to stop it
    /// </summary>
    public GTween Stop()
    {
        tween.Stop();
        return this;
    }

    /// <summary>
    /// Pause the tween
    /// </summary>
    public GTween Pause()
    {
        tween.Pause();
        return this;
    }

    /// <summary>
    /// If the tween was paused with Pause(), resume it with Resume()
    /// </summary>
    public GTween Resume()
    {
        tween.Play();
        return this;
    }

    /// <summary>
    /// Kill the tween
    /// </summary>
    public GTween Kill()
    {
        tween?.Kill();
        return this;
    }

    /// <summary>
    /// Checks if the tween is still playing
    /// </summary>
    public bool IsRunning() => tween.IsRunning();
}

public class GTweenerBuilder
{
    GTween tween;
    PropertyTweener tweener;

    public GTweenerBuilder(GTween tween, PropertyTweener tweener)
    {
        this.tween = tween;
        this.tweener = tweener;
    }

    public GTween Append() => tween;

    public GTweenerBuilder SetTrans(Tween.TransitionType transType)
    {
        tweener.SetTrans(transType);
        return this;
    }

    public GTweenerBuilder SetEase(Tween.EaseType easeType)
    {
        tweener.SetEase(easeType);
        return this;
    }

    public GTweenerBuilder Linear()
    {
        tweener.SetTrans(TransitionType.Linear);
        return this;
    }

    public GTweenerBuilder Back()
    {
        tweener.SetTrans(TransitionType.Back);
        return this;
    }

    public GTweenerBuilder Sine()
    {
        tweener.SetTrans(TransitionType.Sine);
        return this;
    }

    public GTweenerBuilder Bounce()
    {
        tweener.SetTrans(TransitionType.Bounce);
        return this;
    }

    public GTweenerBuilder Circ()
    {
        tweener.SetTrans(TransitionType.Circ);
        return this;
    }

    public GTweenerBuilder Cubic()
    {
        tweener.SetTrans(TransitionType.Cubic);
        return this;
    }

    public GTweenerBuilder Elastic()
    {
        tweener.SetTrans(TransitionType.Elastic);
        return this;
    }

    public GTweenerBuilder Expo()
    {
        tweener.SetTrans(TransitionType.Expo);
        return this;
    }

    public GTweenerBuilder Quad()
    {
        tweener.SetTrans(TransitionType.Quad);
        return this;
    }

    public GTweenerBuilder Quart()
    {
        tweener.SetTrans(TransitionType.Quart);
        return this;
    }

    public GTweenerBuilder Quint()
    {
        tweener.SetTrans(TransitionType.Quint);
        return this;
    }

    public GTweenerBuilder Spring()
    {
        tweener.SetTrans(TransitionType.Spring);
        return this;
    }

    public GTweenerBuilder EaseIn()
    {
        tweener.SetEase(EaseType.In);
        return this;
    }

    public GTweenerBuilder EaseOut()
    {
        tweener.SetEase(EaseType.Out);
        return this;
    }

    public GTweenerBuilder EaseInOut()
    {
        tweener.SetEase(EaseType.InOut);
        return this;
    }

    public GTweenerBuilder EaseOutIn()
    {
        tweener.SetEase(EaseType.OutIn);
        return this;
    }
}
