namespace GodotUtils.Netcode;

public abstract class GamePacket
{
    public    static int MaxSize     { get; } = 8192;
                                     
    protected Peer[]     Peers       { get; set; }
    protected byte       ChannelId   { get; }

    // Packets are reliable by default
    private PacketFlags  PacketFlags { get; set; } = PacketFlags.Reliable;
    private long         Size        { get; set; }
    private byte[]       Data        { get; set; }

    public void Write()
    {
        using (var writer = new PacketWriter())
        {
            writer.Write(GetOpcode());
            this?.Write(writer);

            Data = writer.Stream.ToArray();
            Size = writer.Stream.Length;
        }
    }

    public void SetPeer(Peer peer) => Peers = new Peer[] { peer };
    public void SetPeers(Peer[] peers) => Peers = peers;
    public long GetSize() => Size;
    public abstract byte GetOpcode();
    public virtual void Write(PacketWriter writer) { }
    public virtual void Read(PacketReader reader) { }

    protected ENet.Packet CreateENetPacket()
    {
        var enetPacket = default(ENet.Packet);
        enetPacket.Create(Data, PacketFlags);
        return enetPacket;
    }
}
