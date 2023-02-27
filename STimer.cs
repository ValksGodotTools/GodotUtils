namespace GodotUtils;

using System.Timers;
using Timer = System.Timers.Timer;
using Object = System.Object;

/// <summary>
/// If for whatever reason a Timer is needed on a non-Godot thread, this is what you should use.
/// </summary>
public class STimer : IDisposable
{
    private Timer Timer { get; }

    public STimer(double delayMs, Action action, bool enabled = true, bool autoreset = true)
    {
        void Callback(Object source, ElapsedEventArgs e) => action();
        Timer = new Timer(delayMs);
        Timer.Enabled = enabled;
        Timer.AutoReset = autoreset;
        Timer.Elapsed += Callback;
    }

    public void Start() => Timer.Enabled = true;
    public void Stop() => Timer.Enabled = false;
    public void SetDelay(double delayMs) => Timer.Interval = delayMs;

    public void Dispose() => Timer.Dispose();
}