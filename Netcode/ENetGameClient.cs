using Sandbox2;

namespace GodotUtils.Netcode.Client;

public class ENetGameClient : ENetClient
{
    private ConcurrentQueue<Cmd<GameClientOpcode>> GameClientCmds { get; set; } = new();

    public void EnqueueCmd(GameClientOpcode opcode, params object[] data) =>
        GameClientCmds.Enqueue(new Cmd<GameClientOpcode>(opcode, data));

    protected override void ConcurrentQueues()
    {
        base.ConcurrentQueues();

        while (GameClientCmds.TryDequeue(out Cmd<GameClientOpcode> cmd))
        {

        }
    }
}
