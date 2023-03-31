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

    public void SetDelayMs(int delayMs) => Timer.WaitTime = delayMs / 1000f;

    /// <summary>
    /// Start the timer. Starting the timer while it is active already will reset
    /// the timer countdown.
    /// </summary>
    public void StartMs(float delayMs = -1)
    {
        if (delayMs != -1)
            Timer.WaitTime = delayMs / 1000;

        Timer.Start();
    }

    public void Stop()
    {
        Timer.Stop();

        if (!Callable.Equals(default(Callable)))
            Callable.Call();
    }

    public void QueueFree() => Timer.QueueFree();
}
