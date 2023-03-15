using ENet;

namespace GodotUtils.Netcode.Client;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public class ENetClient<TServerPacketOpcode> : ENetLow
{
	public bool IsConnected => Interlocked.Read(ref _connected) == 1;

	protected ConcurrentQueue<ClientPacket> Outgoing { get; } = new();
	
	private static Dictionary<TServerPacketOpcode, APacketServer> HandlePacket { get; set; }
	private ConcurrentQueue<ENetClientCmd> ENetCmds { get; } = new();
	private Peer Peer { get; set; }
	private uint PingInterval { get; } = 1000;
	private uint PeerTimeout { get; } = 5000;
	private uint PeerTimeoutMinimum { get; } = 5000;
	private uint PeerTimeoutMaximum { get; } = 5000;

	private long _connected;

	public async void Start(string ip, ushort port)
	{
		HandlePacket = NetcodeUtils.LoadInstances<TServerPacketOpcode, APacketServer>("SPacket");
		CTS = new CancellationTokenSource();
		using var task = Task.Run(() => WorkerThread(ip, port), CTS.Token);
		await task;
	}

	public override void Stop()
	{
		Log("Requesting to stop client..");
		ENetCmds.Enqueue(new ENetClientCmd(ENetClientOpcode.Disconnect));
	}

	public void Send<TClientPacketOpcode>(TClientPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
	{
		Outgoing.Enqueue(new ClientPacket(Convert.ToByte(opcode), flags, data));
	}

	protected override void ConcurrentQueues()
	{
		// ENetCmds
		while (ENetCmds.TryDequeue(out ENetClientCmd cmd))
		{
			if (cmd.Opcode == ENetClientOpcode.Disconnect)
			{
				if (CTS.IsCancellationRequested)
				{
					Log("Client is in the middle of stopping");
					break;
				}

				Peer.Disconnect(0);
				DisconnectCleanup(Peer);
			}
		}

		// Outgoing
		while (Outgoing.TryDequeue(out var clientPacket))
		{
			byte channelID = 0; // The channel all networking traffic will be going through
			var packet = default(Packet);
			packet.Create(clientPacket.Data, clientPacket.PacketFlags);
			Peer.Send(channelID, ref packet);
		}
	}

	protected override void Connect(Event netEvent)
	{
		_connected = 1;
		Log("Client connected to server");
	}

	protected override void Disconnect(Event netEvent)
	{
		DisconnectCleanup(Peer);

		var opcode = (DisconnectOpcode)netEvent.Data;
		Log($"Client was {opcode.ToString().ToLower()} from server");
	}

	protected override void Timeout(Event netEvent)
	{
		DisconnectCleanup(Peer);
		Log("Client connection timeout");
	}

	protected override void Receive(Event netEvent)
	{
		var packet = netEvent.Packet;
		if (packet.Length > GamePacket.MaxSize)
		{
			Log($"Tried to read packet from server of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
			packet.Dispose();
			return;
		}

		var packetReader = new PacketReader(packet);
		var opcode = (TServerPacketOpcode)Enum.Parse(typeof(TServerPacketOpcode), packetReader.ReadByte().ToString(), true);
		var handlePacket = HandlePacket[opcode];
		handlePacket.Read(packetReader);

		handlePacket.Handle();

		packetReader.Dispose();
	}

	private void WorkerThread(string ip, ushort port)
	{
		using var client = new Host();
		var address = new Address {
			Port = port
		};

		address.SetHost(ip);
		client.Create();

		Peer = client.Connect(address);
		Peer.PingInterval(PingInterval);
		Peer.Timeout(PeerTimeout, PeerTimeoutMinimum, PeerTimeoutMaximum);

		WorkerLoop(client);

		Log("Client is no longer running");
	}

	protected override void DisconnectCleanup(Peer peer)
	{
		base.DisconnectCleanup(peer);
		_connected = 0;
	}

	public override void Log(object message, ConsoleColor color = ConsoleColor.Cyan) => 
		Logger.Log($"[Client] {message}", color);
}


public class ENetClientCmd
{
	public ENetClientOpcode Opcode { get; set; }
	public object[] Data { get; set; }

	public ENetClientCmd(ENetClientOpcode opcode, params object[] data)
	{
		Opcode = opcode;
		Data = data;
	}
}

public enum ENetClientOpcode
{
	Disconnect
}
