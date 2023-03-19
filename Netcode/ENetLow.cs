namespace GodotUtils.Netcode;

using ENet;
using System.Reflection;

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
    protected List<Type> IgnoredPackets { get; set; } = new();

    protected virtual void DisconnectCleanup(Peer peer)
    {
        CTS.Cancel();
    }

    protected void InitIgnoredPackets<T>(Type[] ignoredPackets)
    {
        IgnoredPackets = ValidateIgnoredPackets<T>(ignoredPackets).ToList();
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

    private List<Type> ValidateIgnoredPackets<T>(Type[] ignoredPackets)
    {
        var serverClient = GetType() == typeof(ENetServer) ? "server" : "client";

        string clientServer;
        if (serverClient == "server")
            clientServer = "client";
        else
            clientServer = "server";

        var ignoredPacketsList = ignoredPackets.ToList();
        var invalidPackets = new List<Type>();

        for (int i = 0; i < ignoredPacketsList.Count; i++)
            if (!typeof(T).IsAssignableFrom(ignoredPacketsList[i]))
            {
                Logger.LogWarning($"The {serverClient} should only ignore {clientServer} " +
                    $"packets but {ignoredPacketsList[i].Name} is a client packet.");

                invalidPackets.Add(ignoredPacketsList[i]);
            }

        foreach (var invalidPacket in invalidPackets)
            ignoredPacketsList.Remove(invalidPacket);

        return ignoredPacketsList;
    }
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
