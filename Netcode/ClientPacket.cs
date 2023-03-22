namespace GodotUtils.Netcode;

public class ClientPacket : GamePacket
{
    public ClientPacket(byte opcode, PacketFlags flags, Peer peer, APacket writable = null)
    {
        using (var writer = new PacketWriter())
        {
            writer.Write(opcode);
            writable?.Write(writer);

            Data = writer.Stream.ToArray();
            Size = writer.Stream.Length;
        }

        Peers = new Peer[] { peer };
        PacketFlags = flags;
        Opcode = opcode;
    }

    public void Send()
    {
        var enetPacket = CreateENetPacket();
        Peers[0].Send(ChannelId, ref enetPacket);
    }
}
