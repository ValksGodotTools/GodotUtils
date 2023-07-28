namespace GodotUtils;

using Godot;
using System;
using Timer = Godot.Timer;

/// <summary>
/// Creates a timer that is non-looping and does not start right away by default
/// </summary>
public class GTimer
{
    public event Action Finished;

    public double TimeLeft => timer.TimeLeft;

    public bool Loop
    {
        get => !timer.OneShot;
        set => timer.OneShot = !value;
    }

    private readonly Timer timer = new();

    public GTimer(Node node, double delayMs = 1000)
    {
        Init(node, delayMs);
        timer.ProcessCallback = Timer.TimerProcessCallback.Physics;
        timer.Timeout += () => Finished?.Invoke();
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
    /// Start the timer. The delay can be optionally changed in ms. 
    /// Starting the timer while it is active already will reset
    /// the timer countdown.
    /// </summary>
    public void Start(float delayMs = -1)
    {
        if (delayMs != -1)
            timer.WaitTime = delayMs / 1000;

        timer.Start();
    }

    /// <summary>
    /// Note that Start() resets the timer wait time so there is no need to worry
    /// about queue freeing the timer
    /// </summary>
    public void Stop()
    {
        timer.Stop();

        // Why did I put this here? I can't remember..
        //if (!callable.Equals(default(Callable)))
        //    callable.Call();

        // Now with new code, above is equivelent to Finished?.Invoke();
        // I'm pretty sure anyways. Hard to know without knowing why I added
        // the above in the first place. To call the callback when timer finished
        // but why?
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
}
