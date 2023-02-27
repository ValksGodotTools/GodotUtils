namespace GodotUtils;

public class GTween 
{
    private Tween Tween { get; }
    private Node Target { get; }

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
