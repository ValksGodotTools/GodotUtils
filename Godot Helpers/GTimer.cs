namespace GodotUtils;

using Godot;
using System;

public class GTimer
{
    public event Action Timeout;

    private Timer timer;

    public GTimer(Node node, double seconds, bool looping)
    {
        timer = new Timer();
        timer.ProcessCallback = Timer.TimerProcessCallback.Physics;
        timer.OneShot = !looping;
        timer.WaitTime = seconds;
        node.AddChild(timer);
        timer.Timeout += () => Timeout?.Invoke();
    }

    public void Start() => timer.Start();
    public void Stop() => timer.Stop();
}
