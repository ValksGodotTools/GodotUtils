namespace GodotUtils.Netcode.Server;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public abstract class ENetServer : ENetLow
{
    private ConcurrentQueue<(Packet, Peer)> Incoming { get; } = new();
    private ConcurrentQueue<ServerPacket> Outgoing { get; } = new();
    private ConcurrentQueue<Cmd<ENetServerOpcode>> ENetCmds { get; } = new();
    public Dictionary<uint, Peer> Peers { get; } = new();
    protected STimer UpdateTimer { get; set; }

    static ENetServer()
    {
        APacketClient.MapOpcodes();
    }

    public ENetServer()
    {
        UpdateTimer = new(100, Update, false);
    }

    protected virtual void Update() { }

    public async void Start(ushort port, int maxClients, params Type[] ignoredPackets)
    {
        Starting();
        InitIgnoredPackets(ignoredPackets);
        UpdateTimer.Start();
        _running = 1;
        CTS = new CancellationTokenSource();
        using var task = Task.Run(() => WorkerThread(port, maxClients), CTS.Token);

        try
        {
            await task;
        } 
        catch (Exception e)
        {
            Logger.LogErr(e, "Server");
        }
    }

    public void Ban(uint id) => Kick(id, DisconnectOpcode.Banned);
    public void BanAll() => KickAll(DisconnectOpcode.Banned);
    public void KickAll(DisconnectOpcode opcode) => 
        ENetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.KickAll, opcode));
    public void Kick(uint id, DisconnectOpcode opcode) =>
        ENetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.Kick, id, opcode));

    public override void Stop()
    {
        Stopping();
        UpdateTimer.Stop();
        UpdateTimer.Dispose();
        Log("Requesting to stop server..");
        ENetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.Stop));
    }

    protected virtual void Stopping() { }

    /// <summary>
    /// Send a packet to a peer
    /// </summary>
    public void Send(APacketServer packet, Peer peer)
    {
        if (!IgnoredPackets.Contains(packet.GetType()))
            Log($"Sending packet {packet.GetType().Name} to peer {peer.ID}\n" +
                $"{packet.PrintFull()}");

        EnqueuePacket(SendType.Peer, packet, peer);
    }

    /// <summary>
    /// If no peers are specified, then the packet will be sent to everyone. If
    /// one peer is specified then that peer will be excluded from the broadcast.
    /// If more than one peer is specified then the packet will only be sent to
    /// those peers.
    /// </summary>
    public void Broadcast(APacketServer packet, params Peer[] peers)
    {
        if (!IgnoredPackets.Contains(packet.GetType()))
        {
            // todo
        }

        EnqueuePacket(SendType.Broadcast, packet, peers);
    }

    private void EnqueuePacket(SendType sendType, APacketServer packet, params Peer[] peers)
    {
        Outgoing.Enqueue(
            new ServerPacket(
                sendType,
                packet.GetOpcode(),
                PacketFlags.Reliable,
                packet,
                peers));
    }

    protected override void ConcurrentQueues()
    {
        // ENet Cmds
        while (ENetCmds.TryDequeue(out Cmd<ENetServerOpcode> cmd))
        {
            if (cmd.Opcode == ENetServerOpcode.Stop)
            {
                if (CTS.IsCancellationRequested)
                {
                    Log("Server is in the middle of stopping");
                    break;
                }

                CTS.Cancel();
            }
            else if (cmd.Opcode == ENetServerOpcode.Kick)
            {
                var id = (uint)cmd.Data[0];
                var opcode = (DisconnectOpcode)cmd.Data[1];

                if (!Peers.ContainsKey(id))
                {
                    Log($"Tried to kick peer with id '{id}' but this peer does not exist");
                    break;
                }

                if (opcode == DisconnectOpcode.Banned)
                {
                    /* 
                     * TODO: Save the peer ip to banned.json and
                     * check banned.json whenever a peer tries to
                     * rejoin
                     */
                }

                Peers[id].DisconnectNow((uint)opcode);
                Peers.Remove(id);
            }
            else if (cmd.Opcode == ENetServerOpcode.KickAll)
            {
                var opcode = (DisconnectOpcode)cmd.Data[0];

                Peers.Values.ForEach(peer =>
                {
                    if (opcode == DisconnectOpcode.Banned)
                    {
                        /* 
                         * TODO: Save the peer ip to banned.json and
                         * check banned.json whenever a peer tries to
                         * rejoin
                         */
                    }

                    peer.DisconnectNow((uint)opcode);
                });
                Peers.Clear();
            }
        }

        // Incoming
        while (Incoming.TryDequeue(out (Packet, Peer) packetPeer))
        {
            var packetReader = new PacketReader(packetPeer.Item1);
            var opcode = packetReader.ReadByte();

            if (!APacketClient.PacketMapBytes.ContainsKey(opcode))
            {
                Log($"Received malformed opcode: {opcode} (Ignoring)");
                return;
            }

            var type = APacketClient.PacketMapBytes[opcode];
            var handlePacket = APacketClient.PacketMap[type].Instance;
            try
            {
                handlePacket.Read(packetReader);
            }
            catch (System.IO.EndOfStreamException e)
            {
                Log($"Received malformed packet: {opcode} {e.Message} (Ignoring)");
                return;
            }
            packetReader.Dispose();

            handlePacket.Handle(packetPeer.Item2);

            if (!IgnoredPackets.Contains(type))
                Log($"Received packet: {type.Name} from peer {packetPeer.Item2.ID}\n" +
                    $"{handlePacket.PrintFull()}");
        }

        // Outgoing
        while (Outgoing.TryDequeue(out ServerPacket packet))
        {
            if (packet.SendType == SendType.Peer)
            {
                packet.Send();
            }
            else if (packet.SendType == SendType.Broadcast)
            {
                packet.Broadcast(Host);
            }
        }
    }

    protected override void Connect(Event netEvent)
    {
        Peers[netEvent.Peer.ID] = netEvent.Peer;
        Log("Client connected - ID: " + netEvent.Peer.ID);
    }

    protected abstract void Disconnected(Event netEvent);

    protected override void Disconnect(Event netEvent)
    {
        Peers.Remove(netEvent.Peer.ID);
        Log("Client disconnected - ID: " + netEvent.Peer.ID);
        Disconnected(netEvent);
    }

    protected override void Timeout(Event netEvent)
    {
        Peers.Remove(netEvent.Peer.ID);
        Log("Client timeout - ID: " + netEvent.Peer.ID);
        Disconnected(netEvent);
    }

    protected override void Receive(Event netEvent)
    {
        var packet = netEvent.Packet;

        if (packet.Length > GamePacket.MaxSize)
        {
            Log($"Tried to read packet from client of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
            packet.Dispose();
            return;
        }

        Incoming.Enqueue((packet, netEvent.Peer));
    }

    private void WorkerThread(ushort port, int maxClients)
    {
        Host = new Host();

        try
        {
            Host.Create(new Address { Port = port }, maxClients);
        }
        catch (InvalidOperationException e)
        {
            Log($"A server is running on port {port} already! {e.Message}");
            return;
        }

        Log("Server is running");

        WorkerLoop();

        Host.Dispose();
        Log("Server is no longer running");
    }

    protected override void DisconnectCleanup(Peer peer)
    {
        base.DisconnectCleanup(peer);
        Peers.Remove(peer.ID);
    }

    public override void Log(object message, ConsoleColor color = ConsoleColor.Green) => 
        Logger.Log($"[Server] {message}", color);
}

public enum ENetServerOpcode
{
    Stop,
    Kick,
    KickAll
}
