namespace GodotUtils.Netcode;

public abstract class GamePacket
{
    public static int MaxSize { get; } = 8192;
                                     
    protected Peer[] Peers { get; set; }
    protected byte ChannelId { get; }

    // Packets are reliable by default
    private readonly PacketFlags packetFlags = PacketFlags.Reliable;
    private long size;
    private byte[] data;

    public void Write()
    {
        using (var writer = new PacketWriter())
        {
            writer.Write(GetOpcode());
            this?.Write(writer);

            data = writer.Stream.ToArray();
            size = writer.Stream.Length;
        }
    }

    public void SetPeer(Peer peer) => Peers = new Peer[] { peer };
    public void SetPeers(Peer[] peers) => Peers = peers;
    public long GetSize() => size;
    public abstract byte GetOpcode();
    public virtual void Write(PacketWriter writer) { }
    public virtual void Read(PacketReader reader) { }

    protected ENet.Packet CreateENetPacket()
    {
        var enetPacket = default(ENet.Packet);
        enetPacket.Create(data, packetFlags);
        return enetPacket;
    }
}
