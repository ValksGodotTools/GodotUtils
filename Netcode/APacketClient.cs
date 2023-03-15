namespace GodotUtils.Netcode;

public abstract class APacketClient : APacket
{
	public static Dictionary<Type, PacketInfo<APacketClient>> PacketMap { get; } = NetcodeUtils.MapPackets<APacketClient>();
	public static Dictionary<byte, Type> PacketMapBytes { get; set; } = new();

	public static void MapOpcodes()
	{
		foreach (var packet in PacketMap)
			PacketMapBytes.Add(packet.Value.Opcode, packet.Key);
	}

	public override byte GetOpcode() => PacketMap[GetType()].Opcode;

	/// <summary>
	/// The packet handled server-side
	/// </summary>
	/// <param name="peer">The client peer</param>
	public abstract void Handle(ENet.Peer peer);
}
