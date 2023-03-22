namespace GodotUtils.Netcode;

public class ServerPacket : GamePacket
{
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

        if (Peers.Count() == 0)
        {
            host.Broadcast(ChannelId, ref enetPacket);
        }
        else if (Peers.Count() == 1)
        {
            host.Broadcast(ChannelId, ref enetPacket, Peers[0]);
        }
        else
        {
            host.Broadcast(ChannelId, ref enetPacket, Peers);
        }
    }
}

public enum SendType
{
    Peer,
    Broadcast
}
