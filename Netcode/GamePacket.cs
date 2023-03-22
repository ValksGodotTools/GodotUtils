namespace GodotUtils.Netcode;

public abstract class GamePacket
{
    public static int MaxSize { get; } = 8192;
    public Peer[] Peers { get; set; }
    public byte Opcode { get; set; }
    public PacketFlags PacketFlags { get; set; } = PacketFlags.Reliable; // Lets make packets reliable by default
    public byte[] Data { get; set; }
    public long Size { get; set; }

    protected byte ChannelId { get; }

    protected ENet.Packet CreateENetPacket()
    {
        var enetPacket = default(Packet);
        enetPacket.Create(Data, PacketFlags);
        return enetPacket;
    }
}
