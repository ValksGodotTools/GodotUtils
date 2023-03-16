namespace GodotUtils.Netcode;

public abstract class APacket
{
    public abstract byte GetOpcode();
    public virtual void Write(PacketWriter writer) { }
    public virtual void Read(PacketReader reader) { }
}
