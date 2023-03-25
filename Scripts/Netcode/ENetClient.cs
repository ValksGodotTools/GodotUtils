namespace GodotUtils.Netcode.Client;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public abstract class ENetClient : ENetLow
{
    private   ConcurrentQueue<ENet.Packet>           Incoming     { get; } = new();
    protected ConcurrentQueue<ClientPacket>          Outgoing     { get; } = new();
    private   ConcurrentQueue<PacketData>            GodotPackets { get; } = new();
    public    ConcurrentQueue<Cmd<GodotOpcode>>      GodotCmds    { get; } = new();
    private   ConcurrentQueue<Cmd<ENetClientOpcode>> ENetCmds     { get; } = new();

    private ENetOptions Options            { get; set; }
    private Peer        Peer               { get; set; }
    private uint        PingInterval       { get; } = 1000;
    private uint        PeerTimeout        { get; } = 5000;
    private uint        PeerTimeoutMinimum { get; } = 5000;
    private uint        PeerTimeoutMaximum { get; } = 5000;

    private long _connected;

    static ENetClient()
    {
        ServerPacket.MapOpcodes();
    }

    public bool IsConnected => Interlocked.Read(ref _connected) == 1;

    public async void Connect(string ip, ushort port, ENetOptions options = default, params Type[] ignoredPackets)
    {
        Options = options;
        Starting();
        InitIgnoredPackets(ignoredPackets);

        _running = 1;
        CTS = new CancellationTokenSource();
        using var task = Task.Run(() => WorkerThread(ip, port), CTS.Token);

        try
        {
            await task;
        }
        catch (Exception e)
        {
            Logger.LogErr(e, "Client");
        }
    }

    public override void Stop()
    {
        ENetCmds.Enqueue(new Cmd<ENetClientOpcode>(ENetClientOpcode.Disconnect));
    }

    public void Send(ClientPacket packet, PacketFlags flags = PacketFlags.Reliable)
    {
        if (!IsConnected)
            return;

        packet.Write();
        packet.SetPeer(Peer);
        Outgoing.Enqueue(packet);
    }

    protected override void ConcurrentQueues()
    {
        // ENetCmds
        while (ENetCmds.TryDequeue(out Cmd<ENetClientOpcode> cmd))
        {
            if (cmd.Opcode == ENetClientOpcode.Disconnect)
            {
                if (CTS.IsCancellationRequested)
                {
                    Log("Client is in the middle of stopping");
                    break;
                }

                Peer.Disconnect(0);
                DisconnectCleanup(Peer);
            }
        }

        // Incoming
        while (Incoming.TryDequeue(out var packet))
        {
            var packetReader = new PacketReader(packet);
            var opcode = packetReader.ReadByte();

            var type = ServerPacket.PacketMapBytes[opcode];
            var handlePacket = ServerPacket.PacketMap[type].Instance;

            /*
            * Instead of packets being handled client-side, they are handled
            * on the Godot thread.
            * 
            * Note that handlePacket AND packetReader need to be sent over otherwise
            * the following issue will happen...
            * https://github.com/Valks-Games/Multiplayer-Template/issues/8
            */
            GodotPackets.Enqueue(new PacketData
            {
                Type = type,
                PacketReader = packetReader,
                HandlePacket = handlePacket
            });
        }

        // Outgoing
        while (Outgoing.TryDequeue(out var clientPacket))
        {
            var type = clientPacket.GetType();

            if (!IgnoredPackets.Contains(type) && Options.PrintPacketSent)
                Log($"Sent packet: {type.Name}" +
                    $"{(Options.PrintPacketData ? $"\n{clientPacket.PrintFull()}" : "")}");

            clientPacket.Send();
        }
    }

    public void HandlePackets()
    {
        while (GodotPackets.TryDequeue(out PacketData packetData))
        {
            var packetReader = packetData.PacketReader;
            var handlePacket = packetData.HandlePacket;
            var type = packetData.Type;

            handlePacket.Read(packetReader);
            packetReader.Dispose();

            handlePacket.Handle();

            if (!IgnoredPackets.Contains(type) && Options.PrintPacketReceived)
                Log($"Received packet: {type.Name}" +
                    $"{(Options.PrintPacketData ? $"\n{handlePacket.PrintFull()}" : "")}");
        }
    }

    protected override void Connect(Event netEvent)
    {
        _connected = 1;
        Log("Client connected to server");
    }

    protected override void Disconnect(Event netEvent)
    {
        DisconnectCleanup(Peer);

        var opcode = (DisconnectOpcode)netEvent.Data;
        Log($"Received disconnect opcode from server: {opcode.ToString().ToLower()}");
        GodotCmds.Enqueue(new Cmd<GodotOpcode>(GodotOpcode.Disconnected, opcode));
    }

    protected override void Timeout(Event netEvent)
    {
        DisconnectCleanup(Peer);
        Log("Client connection timeout");
    }

    protected override void Receive(Event netEvent)
    {
        var packet = netEvent.Packet;
        if (packet.Length > GamePacket.MaxSize)
        {
            Log($"Tried to read packet from server of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
            packet.Dispose();
            return;
        }

        Incoming.Enqueue(packet);
    }

    private void WorkerThread(string ip, ushort port)
    {
        Host = new Host();
        var address = new Address {
            Port = port
        };

        address.SetHost(ip);
        Host.Create();

        Peer = Host.Connect(address);
        Peer.PingInterval(PingInterval);
        Peer.Timeout(PeerTimeout, PeerTimeoutMinimum, PeerTimeoutMaximum);

        WorkerLoop();

        Host.Dispose();
        Log("Client is no longer running");
    }

    protected override void DisconnectCleanup(Peer peer)
    {
        base.DisconnectCleanup(peer);
        _connected = 0;
    }

    public override void Log(object message, ConsoleColor color = ConsoleColor.Cyan) => 
        Logger.Log($"[Client] {message}", color);
}

public enum ENetClientOpcode
{
    Disconnect
}

public enum GodotOpcode
{
    Disconnected
}

public class PacketData
{
    public Type Type { get; set; }
    public PacketReader PacketReader { get; set; }
    public ServerPacket HandlePacket { get; set; }
}
