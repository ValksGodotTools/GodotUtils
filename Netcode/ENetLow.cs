namespace GodotUtils.Netcode;

using ENet;

public abstract class ENetLow
{
    public static bool ENetInitialized { get; set; }

    static ENetLow()
    {
        try
        {
            Library.Initialize();
            ENetInitialized = true;
        }
        catch (DllNotFoundException e)
        {
            Logger.LogErr(e);
            ENetInitialized = false;
        }
    }

    public bool IsRunning => Interlocked.Read(ref _running) == 1;
    public abstract void Log(object message, ConsoleColor color);
    public abstract void Stop();

    protected CancellationTokenSource CTS { get; set; }

    protected virtual void DisconnectCleanup(Peer peer)
    {
        CTS.Cancel();
    }

    protected void WorkerLoop(Host host)
    {
        while (!CTS.IsCancellationRequested)
        {
            var polled = false;

            ConcurrentQueues();

            while (!polled)
            {
                if (host.CheckEvents(out Event netEvent) <= 0)
                {
                    if (host.Service(15, out netEvent) <= 0)
                        break;

                    polled = true;
                }

                switch (netEvent.Type)
                {
                    case EventType.None:
                        // do nothing
                        break;
                    case EventType.Connect:
                        Connect(netEvent);
                        break;
                    case EventType.Disconnect:
                        Disconnect(netEvent);
                        break;
                    case EventType.Timeout:
                        Timeout(netEvent);
                        break;
                    case EventType.Receive:
                        Receive(netEvent);
                        break;
                }
            }
        }

        host.Flush();
        _running = 0;
        Stopped();
    }

    protected abstract void Stopped();
    protected abstract void Connect(Event netEvent);
    protected abstract void Disconnect(Event netEvent);
    protected abstract void Timeout(Event netEvent);
    protected abstract void Receive(Event netEvent);
    protected abstract void ConcurrentQueues();

    protected long _running;
}

public enum DisconnectOpcode
{
    Disconnected,
    Maintenance,
    Restarting,
    Stopping,
    Kicked,
    Banned
}
