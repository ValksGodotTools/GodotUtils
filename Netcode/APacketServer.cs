using GodotUtils.Netcode.Client;

namespace GodotUtils.Netcode;

public abstract class APacketServer : APacket
{
    /// <summary>
    /// The packet handled client-side (Godot thread)
    /// </summary>
    public abstract void Handle();
}
