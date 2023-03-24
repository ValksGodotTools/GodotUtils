using Timer = Godot.Timer;

namespace GodotUtils;

public class GTimer
{
    private Timer Timer { get; } = new();
    private Callable Callable { get; }

    public double TimeLeft
    {
        get { return Timer.TimeLeft; }
    }

    public bool Loop
    {
        get { return !Timer.OneShot; }
        set { Timer.OneShot = !value; }
    }

    public GTimer(Node node, double delayMs = 1000) =>
        Init(node, delayMs);

    public GTimer(Node node, Action action, double delayMs = 1000)
    {
        Init(node, delayMs);
        Callable = Callable.From(action);
        Timer.Connect("timeout", Callable);
    }

    private void Init(Node target, double delayMs)
    {
        Timer.OneShot = true; // make non-looping by default
        Timer.Autostart = false; // make non-auto-start by default
        Timer.WaitTime = delayMs / 1000;
        target.AddChild(Timer);
    }

    public bool IsActive() => Timer.TimeLeft != 0;

    public void SetDelay(float delay) => Timer.WaitTime = delay;

    public void SetDelayMs(int delayMs) => Timer.WaitTime = delayMs / 1000f;

    public void Start(float delay)
    {
        Timer.WaitTime = delay;
        Start();
    }

    public void StartMs(float delayMs)
    {
        Timer.WaitTime = delayMs / 1000;
        Start();
    }

    public void Start() => Timer.Start();

    public void Stop()
    {
        Timer.Stop();

        if (!Callable.Equals(default(Callable)))
            Callable.Call();
    }

    public void QueueFree() => Timer.QueueFree();
}
