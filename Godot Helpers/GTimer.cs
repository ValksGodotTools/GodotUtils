namespace GodotUtils;

using Timer = Godot.Timer;

/// <summary>
/// Creates a timer that is non-looping and does not start right away by default
/// </summary>
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

    public void SetDelay(int delayMs) => timer.WaitTime = delayMs / 1000f;

    /// <summary>
    /// Start the timer. Starting the timer while it is active already will reset
    /// the timer countdown.
    /// </summary>
    public void Start(float delayMs = -1)
    {
        if (delayMs != -1)
            timer.WaitTime = delayMs / 1000;

        timer.Start();
    }

    public void Stop()
    {
        timer.Stop();

        // Why did I put this here? I can't remember..
        //if (!callable.Equals(default(Callable)))
        //    callable.Call();
    }

    public void QueueFree() => timer.QueueFree();
}

/// <summary>
/// Creates a timer that only loops once and starts right away
/// </summary>
public class GOneShotTimer : GTimer
{
    public GOneShotTimer(Node node, double delayMs = 1000) : base(node, delayMs)
    {
        Loop = false;
        Start();
    }

    public GOneShotTimer(Node node, Action action, double delayMs = 1000) : base(node, action, delayMs)
    {
        Loop = false;
        Start();
    }
}

/// <summary>
/// Creates a timer that loops and starts right away
/// </summary>
public class GRepeatingTimer : GTimer
{
    public GRepeatingTimer(Node node, double delayMs = 1000) : base(node, delayMs)
    {
        Loop = true;
        Start();
    }

    public GRepeatingTimer(Node node, Action action, double delayMs = 1000) : base(node, action, delayMs)
    {
        Loop = true;
        Start();
    }
}
