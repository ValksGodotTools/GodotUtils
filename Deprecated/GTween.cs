namespace GodotUtils.Deprecated;

using Godot;
using System.Threading.Tasks;

/*
 * I feel like I could do a much better job with creating a more generic util
 * for something like this so that is why this is being deprecated but not being
 * deleted outright because Sankari code is using this and if I delete this I feel
 * like I'm going to spend a long time trying to figure out what I did to make the
 * code work.
 */
public class GTween 
{
    Tween Tween { get; }
    Node Target { get; }

    public GTween(Node target)
    {
        this.Target = target;
        Tween = this.Target.GetTree().CreateTween();
        Tween.Stop();
    }

    public void Callback(string methodName) => Tween.TweenCallback(new Callable(Target, methodName));

    /// <summary>
    /// Hover over the property in the editor to get the string value of that property.
    /// </summary>
    public PropertyTweener InterpolateProperty
    (
        NodePath property, 
        Variant finalValue, 
        float duration,
        float delay = 0,
        bool parallel = false,
        Tween.EaseType easeType = Tween.EaseType.InOut,
        Tween.TransitionType transType = Tween.TransitionType.Quad
    )
    {
        if (parallel)
            return Tween.Parallel().TweenProperty(Target, property, finalValue, duration)
                .SetEase(easeType)
                .SetTrans(transType)
                .SetDelay(delay);
        else
            return Tween.TweenProperty(Target, property, finalValue, duration)
                .SetEase(easeType)
                .SetTrans(transType)
                .SetDelay(delay);
    }

    public async Task AnimatePlatform
    (
        Vector2 initialValue, 
        Vector2 finalValue, 
        float width,
        float duration,
        int startDelay,
        Tween.TransitionType transType = Tween.TransitionType.Cubic,
        Tween.EaseType easeType = Tween.EaseType.InOut
    ) 
    {
        Tween.SetLoops(); // Run forever
        InterpolateProperty("position", finalValue, duration);
        InterpolateProperty("position", initialValue, duration);
        await Task.Delay(startDelay * 1000);
        Start();
    }

    public void IsActive() => Tween.IsRunning();
    public void Start() => Tween.Play();
    public void Pause() => Tween.Stop();
}
