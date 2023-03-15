using GodotUtils.Netcode.Client;

namespace GodotUtils.Netcode;

public abstract class APacketServer : APacket
{
    public static Dictionary<Type, PacketInfo<APacketServer>> PacketMap { get; } = NetcodeUtils.MapPackets<APacketServer>();
    public static Dictionary<byte, Type> PacketMapBytes { get; set; } = new();

    public static void MapOpcodes()
    {
        foreach (var packet in PacketMap)
            PacketMapBytes.Add(packet.Value.Opcode, packet.Key);
    }

    public override byte GetOpcode() => PacketMap[GetType()].Opcode;

    /// <summary>
    /// The packet handled client-side (Godot thread)
    /// </summary>
    public abstract void Handle();
}
