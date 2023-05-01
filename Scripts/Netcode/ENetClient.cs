namespace GodotUtils.Netcode.Client;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public abstract class ENetClient : ENetLow
{
    public ConcurrentQueue<Cmd<GodotOpcode>> GodotCmds { get; } = new();
    protected ConcurrentQueue<ClientPacket> Outgoing { get; } = new();

    private const uint PING_INTERVAL = 1000;
    private const uint PEER_TIMEOUT = 5000;
    private const uint PEER_TIMEOUT_MINIMUM = 5000;
    private const uint PEER_TIMEOUT_MAXIMUM = 5000;

    private readonly ConcurrentQueue<PacketData> godotPackets = new();
    private readonly ConcurrentQueue<ENet.Packet> incoming = new();
    private readonly ConcurrentQueue<Cmd<ENetClientOpcode>> eNetCmds = new();

    private ENetOptions options;
    private Peer peer;
    private long connected;

    static ENetClient()
    {
        ServerPacket.MapOpcodes();
    }

    public bool IsConnected => Interlocked.Read(ref connected) == 1;

    public async void Connect(string ip, ushort port, ENetOptions options = default, params Type[] ignoredPackets)
    {
        this.options = options;
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
        eNetCmds.Enqueue(new Cmd<ENetClientOpcode>(ENetClientOpcode.Disconnect));
    }

    public void Send(ClientPacket packet, PacketFlags flags = PacketFlags.Reliable)
    {
        if (!IsConnected)
            return;

        packet.Write();
        packet.SetPeer(peer);
        Outgoing.Enqueue(packet);
    }

    protected override void ConcurrentQueues()
    {
        // ENetCmds
        while (eNetCmds.TryDequeue(out Cmd<ENetClientOpcode> cmd))
        {
            if (cmd.Opcode == ENetClientOpcode.Disconnect)
            {
                if (CTS.IsCancellationRequested)
                {
                    Log("Client is in the middle of stopping");
                    break;
                }

                peer.Disconnect(0);
                DisconnectCleanup(peer);
            }
        }

        // Incoming
        while (incoming.TryDequeue(out var packet))
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
            godotPackets.Enqueue(new PacketData
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

            if (!IgnoredPackets.Contains(type) && options.PrintPacketSent)
                Log($"Sent packet: {type.Name}" +
                    $"{(options.PrintPacketData ? $"\n{clientPacket.PrintFull()}" : "")}");

            clientPacket.Send();
        }
    }

    public void HandlePackets()
    {
        while (godotPackets.TryDequeue(out PacketData packetData))
        {
            var packetReader = packetData.PacketReader;
            var handlePacket = packetData.HandlePacket;
            var type = packetData.Type;

            handlePacket.Read(packetReader);
            packetReader.Dispose();

            handlePacket.Handle();

            if (!IgnoredPackets.Contains(type) && options.PrintPacketReceived)
                Log($"Received packet: {type.Name}" +
                    $"{(options.PrintPacketData ? $"\n{handlePacket.PrintFull()}" : "")}", LoggerColor.Deepskyblue);
        }
    }

    protected override void Connect(Event netEvent)
    {
        connected = 1;
        Log("Client connected to server");
    }

    protected override void Disconnect(Event netEvent)
    {
        DisconnectCleanup(peer);

        var opcode = (DisconnectOpcode)netEvent.Data;
        Log($"Received disconnect opcode from server: {opcode.ToString().ToLower()}");
        GodotCmds.Enqueue(new Cmd<GodotOpcode>(GodotOpcode.Disconnected, opcode));
    }

    protected override void Timeout(Event netEvent)
    {
        DisconnectCleanup(peer);
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

        incoming.Enqueue(packet);
    }

    private void WorkerThread(string ip, ushort port)
    {
        Host = new Host();
        var address = new Address {
            Port = port
        };

        address.SetHost(ip);
        Host.Create();

        peer = Host.Connect(address);
        peer.PingInterval(PING_INTERVAL);
        peer.Timeout(PEER_TIMEOUT, PEER_TIMEOUT_MINIMUM, PEER_TIMEOUT_MAXIMUM);

        WorkerLoop();

        Host.Dispose();
        Log("Client is no longer running");
    }

    protected override void DisconnectCleanup(Peer peer)
    {
        base.DisconnectCleanup(peer);
        connected = 0;
    }

    public override void Log(object message, LoggerColor color = LoggerColor.Aqua) => 
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
