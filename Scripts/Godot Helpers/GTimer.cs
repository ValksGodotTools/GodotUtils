namespace GodotUtils;

using Timer = Godot.Timer;

public class GTimer
{
    private readonly Timer timer = new();
    private Callable callable;

    public double TimeLeft
    {
        get => timer.TimeLeft;
    }

    public bool Loop
    {
        get => !timer.OneShot;
        set => timer.OneShot = !value;
    }

    public GTimer(Node node, double delayMs = 1000) =>
        Init(node, delayMs);

    public GTimer(Node node, Action action, double delayMs = 1000)
    {
        Init(node, delayMs);
        callable = Callable.From(action);
        timer.Connect("timeout", callable);
    }

    private void Init(Node target, double delayMs)
    {
        timer.OneShot = true; // make non-looping by default
        timer.Autostart = false; // make non-auto-start by default
        timer.WaitTime = delayMs / 1000;
        target.AddChild(timer);
    }

    public bool IsActive() => timer.TimeLeft != 0;

    public void SetDelayMs(int delayMs) => timer.WaitTime = delayMs / 1000f;

    /// <summary>
    /// Start the timer. Starting the timer while it is active already will reset
    /// the timer countdown.
    /// </summary>
    public void StartMs(float delayMs = -1)
    {
        if (delayMs != -1)
            timer.WaitTime = delayMs / 1000;

        timer.Start();
    }

    public void Stop()
    {
        timer.Stop();

        if (!callable.Equals(default(Callable)))
            callable.Call();
    }

    public void QueueFree() => timer.QueueFree();
}
