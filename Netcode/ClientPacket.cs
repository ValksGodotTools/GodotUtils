namespace GodotUtils.Netcode;

public class ClientPacket : GamePacket
{
    public ClientPacket(byte opcode, PacketFlags flags, APacket writable = null)
    {
        using (var writer = new PacketWriter())
        {
            writer.Write(opcode);
            writable?.Write(writer);

            Data = writer.Stream.ToArray();
            Size = writer.Stream.Length;
        }

        PacketFlags = flags;
        Opcode = opcode;
    }

    public void Send(Peer peer)
    {
        var enetPacket = CreateENetPacket();
        peer.Send(ChannelId, ref enetPacket);
    }
}
