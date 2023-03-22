namespace GodotUtils.Netcode;

public class ServerPacket : GamePacket
{
    public Peer[] Peers { get; }
    public SendType SendType { get; }

    public ServerPacket(SendType sendType, byte opcode, PacketFlags packetFlags, APacket writable = null, params Peer[] peers)
    {
        using (var writer = new PacketWriter())
        {
            writer.Write(opcode);
            writable?.Write(writer);

            Data = writer.Stream.ToArray();
            Size = writer.Stream.Length;
        }

        SendType = sendType;
        Opcode = opcode;
        PacketFlags = packetFlags;
        Peers = peers;
    }

    public void Send()
    {
        var enetPacket = CreateENetPacket();
        Peers[0].Send(ChannelId, ref enetPacket);
    }

    public void Broadcast(Host host)
    {
        var enetPacket = CreateENetPacket();
        var peers = Peers;

        if (peers.Count() == 0)
        {
            host.Broadcast(ChannelId, ref enetPacket);
        }
        else if (peers.Count() == 1)
        {
            host.Broadcast(ChannelId, ref enetPacket, peers[0]);
        }
        else
        {
            host.Broadcast(ChannelId, ref enetPacket, peers);
        }
    }
}

public enum SendType
{
    Peer,
    Broadcast
}
