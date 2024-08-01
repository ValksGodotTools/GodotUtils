namespace GodotUtils.Netcode.Server;

using ENet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public abstract class ENetServer : ENetLow
{
    public Dictionary<uint, Peer> Peers { get; } = new();
    protected STimer EmitLoop { get; set; }

    readonly ConcurrentQueue<(Packet, Peer)> incoming = new();
    readonly ConcurrentQueue<ServerPacket> outgoing = new();
    readonly ConcurrentQueue<Cmd<ENetServerOpcode>> enetCmds = new();

    static ENetServer()
    {
        ClientPacket.MapOpcodes();
    }

    public ENetServer()
    {
        EmitLoop = new(100, Emit, false);
    }

    protected virtual void Emit() { }

    public async void Start(ushort port, int maxClients, ENetOptions options, params Type[] ignoredPackets)
    {
        this.options = options;
        Starting();
        InitIgnoredPackets(ignoredPackets);
        EmitLoop.Start();
        _running = 1;
        CTS = new CancellationTokenSource();
        using Task task = Task.Run(() => WorkerThread(port, maxClients), CTS.Token);

        try
        {
            await task;
        }
        catch (Exception e)
        {
            GU.Services.Get<Logger>().LogErr(e, "Server");
        }
    }

    public void Ban(uint id) => Kick(id, DisconnectOpcode.Banned);
    public void BanAll() => KickAll(DisconnectOpcode.Banned);
    public void KickAll(DisconnectOpcode opcode) =>
        enetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.KickAll, opcode));
    public void Kick(uint id, DisconnectOpcode opcode) =>
        enetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.Kick, id, opcode));

    public override void Stop()
    {
        Stopping();
        EmitLoop.Stop();
        EmitLoop.Dispose();
        enetCmds.Enqueue(new Cmd<ENetServerOpcode>(ENetServerOpcode.Stop));
    }

    protected virtual void Stopping() { }

    /// <summary>
    /// Send a packet to a peer
    /// </summary>
    public void Send(ServerPacket packet, Peer peer)
    {
        packet.Write();

        Type type = packet.GetType();

        if (!IgnoredPackets.Contains(type) && options.PrintPacketSent)
            Log($"Sending packet {type.Name} {(options.PrintPacketByteSize ? $"({packet.GetSize()} bytes)" : "")}to peer {peer.ID}" +
                $"{(options.PrintPacketData ? $"\n{packet.PrintFull()}" : "")}");

        packet.SetSendType(SendType.Peer);
        packet.SetPeer(peer);

        EnqueuePacket(packet);
    }

    /// <summary>
    /// If no peers are specified, then the packet will be sent to everyone. If
    /// one peer is specified then that peer will be excluded from the broadcast.
    /// If more than one peer is specified then the packet will only be sent to
    /// those peers.
    /// </summary>
    public void Broadcast(ServerPacket packet, params Peer[] peers)
    {
        packet.Write();

        Type type = packet.GetType();

        if (!IgnoredPackets.Contains(type) && options.PrintPacketSent)
        {
            // This is messy but I don't know how I will clean it up right
            // now so I'm leaving it as is for now..
            string byteSize = options.PrintPacketByteSize ?
                $"({packet.GetSize()} bytes)" : "";

            string start = $"Broadcasting packet {type.Name} {byteSize}";

            string peerArr = peers.Select(x => x.ID).Print();

            string middle = "";

            string end = options.PrintPacketData ?
                $"\n{packet.PrintFull()}" : "";

            if (peers.Count() == 0)
                middle = "to everyone";

            else if (peers.Count() == 1)
                middle = $"to everyone except peer {peerArr}";

            else
                middle = $"to peers {peerArr}";

            string message = start + middle + end;

            Log(message);
        }

        packet.SetSendType(SendType.Broadcast);
        packet.SetPeers(peers);

        EnqueuePacket(packet);
    }

    void EnqueuePacket(ServerPacket packet)
    {
        outgoing.Enqueue(packet);
    }

    protected override void ConcurrentQueues()
    {
        // ENet Cmds
        while (enetCmds.TryDequeue(out Cmd<ENetServerOpcode> cmd))
        {
            if (cmd.Opcode == ENetServerOpcode.Stop)
            {
                KickAll(DisconnectOpcode.Stopping);

                if (CTS.IsCancellationRequested)
                {
                    Log("Server is in the middle of stopping");
                    break;
                }

                CTS.Cancel();
            }
            else if (cmd.Opcode == ENetServerOpcode.Kick)
            {
                uint id = (uint)cmd.Data[0];
                DisconnectOpcode opcode = (DisconnectOpcode)cmd.Data[1];

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
                DisconnectOpcode opcode = (DisconnectOpcode)cmd.Data[0];

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
        while (incoming.TryDequeue(out (ENet.Packet, Peer) packetPeer))
        {
            PacketReader packetReader = new PacketReader(packetPeer.Item1);
            byte opcode = packetReader.ReadByte();

            if (!ClientPacket.PacketMapBytes.ContainsKey(opcode))
            {
                Log($"Received malformed opcode: {opcode} (Ignoring)");
                return;
            }

            Type type = ClientPacket.PacketMapBytes[opcode];
            ClientPacket handlePacket = ClientPacket.PacketMap[type].Instance;
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

            handlePacket.Handle(this, packetPeer.Item2);

            if (!IgnoredPackets.Contains(type) && options.PrintPacketReceived)
                Log($"Received packet: {type.Name} from peer {packetPeer.Item2.ID}" +
                    $"{(options.PrintPacketData ? $"\n{handlePacket.PrintFull()}" : "")}", BBColor.LightGreen);
        }

        // Outgoing
        while (outgoing.TryDequeue(out ServerPacket packet))
        {
            SendType sendType = packet.GetSendType();

            if (sendType == SendType.Peer)
            {
                packet.Send();
            }
            else if (sendType == SendType.Broadcast)
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
        ENet.Packet packet = netEvent.Packet;

        if (packet.Length > GamePacket.MaxSize)
        {
            Log($"Tried to read packet from client of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
            packet.Dispose();
            return;
        }

        incoming.Enqueue((packet, netEvent.Peer));
    }

    void WorkerThread(ushort port, int maxClients)
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

    public override void Log(object message, BBColor color = BBColor.Green) =>
        GU.Services.Get<Logger>().Log($"[Server] {message}", color);
}

public enum ENetServerOpcode
{
    Stop,
    Kick,
    KickAll
}
