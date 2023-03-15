namespace GodotUtils.Netcode;

public abstract class ENetLow
{
	public abstract void Log(object message, ConsoleColor color);

	protected CancellationTokenSource CTS { get; set; }
	protected abstract void DisconnectCleanup();
	protected abstract void Stop();
}
