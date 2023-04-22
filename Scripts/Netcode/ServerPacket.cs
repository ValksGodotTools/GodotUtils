namespace GodotUtils.Netcode;

using GodotUtils.Netcode.Client;

public abstract class ServerPacket : GamePacket
{
    public static Dictionary<Type, PacketInfo<ServerPacket>> PacketMap { get; } = NetcodeUtils.MapPackets<ServerPacket>();
    public static Dictionary<byte, Type> PacketMapBytes { get; set; } = new();

    private SendType SendType { get; set; }

    public static void MapOpcodes()
    {
        foreach (var packet in PacketMap)
            PacketMapBytes.Add(packet.Value.Opcode, packet.Key);
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

    public void SetSendType(SendType sendType) => SendType = sendType;
    public SendType GetSendType() => SendType;

    public override byte GetOpcode() => PacketMap[GetType()].Opcode;

    /// <summary>
    /// The packet handled client-side (Godot thread)
    /// </summary>
    public abstract void Handle();
}

public enum SendType
{
    Peer,
    Broadcast
}
