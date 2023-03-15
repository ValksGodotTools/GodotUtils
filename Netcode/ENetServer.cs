using ENet;

namespace GodotUtils.Netcode.Server;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public class ENetServer<TClientPacketOpcode> : ENetLow
{
	private static Dictionary<TClientPacketOpcode, APacketClient> HandlePacket { get; set; } = NetcodeUtils.LoadInstances<TClientPacketOpcode, APacketClient>("CPacket");
	private ConcurrentQueue<ServerPacket> Outgoing { get; } = new();
	private ConcurrentQueue<ENetServerCmd> ENetCmds { get; } = new();

	public async void Start(ushort port, int maxClients)
	{
		CTS = new CancellationTokenSource();
		using var task = Task.Run(() => WorkerThread(port, maxClients), CTS.Token);
		await task;
	}

	public override void Stop()
	{
		Log("Requesting to stop server..");
		ENetCmds.Enqueue(new ENetServerCmd(ENetServerOpcode.Stop));
	}

	public void Send<TServerPacketOpcode>(TServerPacketOpcode opcode, APacket packet, Peer peer, params Peer[] peers) =>
		Outgoing.Enqueue(new ServerPacket(Convert.ToByte(opcode), PacketFlags.Reliable, packet, JoinPeers(peer, peers)));

	private void WorkerThread(ushort port, int maxClients)
	{
		using var server = new Host();

		var address = new Address {
			Port = port
		};

		try
		{
			server.Create(address, maxClients);
		}
		catch (InvalidOperationException e)
		{
			Log($"A server is running on port {port} already! {e.Message}");
			return;
		}

		while (!CTS.IsCancellationRequested)
		{
			var polled = false;

			// ENet Cmds
			while (ENetCmds.TryDequeue(out ENetServerCmd cmd))
			{
				switch (cmd.Opcode)
				{
					case ENetServerOpcode.Stop:
						if (CTS.IsCancellationRequested)
						{
							Log("Server is in the middle of stopping");
							break;
						}

						DisconnectCleanup();
						break;
				}
			}

			// Outgoing
			while (Outgoing.TryDequeue(out ServerPacket packet))
				packet.Peers.ForEach(peer => Send(packet, peer));

			while (!polled)
			{
				if (server.CheckEvents(out Event netEvent) <= 0)
				{
					if (server.Service(15, out netEvent) <= 0)
						break;

					polled = true;
				}

				switch (netEvent.Type)
				{
					case EventType.None:
						break;

					case EventType.Connect:
						Log("Client connected - ID: " + netEvent.Peer.ID);
						break;

					case EventType.Disconnect:
						DisconnectCleanup();
						Log("Client disconnected - ID: " + netEvent.Peer.ID);
						break;

					case EventType.Timeout:
						DisconnectCleanup();
						Log("Client timeout - ID: " + netEvent.Peer.ID);
						break;

					case EventType.Receive:
						var packet = netEvent.Packet;

						if (packet.Length > GamePacket.MaxSize)
						{
							Log($"Tried to read packet from client of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
							packet.Dispose();
							continue;
						}

						var packetReader = new PacketReader(packet);
						var opcode = (TClientPacketOpcode)Enum.Parse(typeof(TClientPacketOpcode), packetReader.ReadByte().ToString(), true);

						if (!HandlePacket.ContainsKey(opcode)) 
						{ 
							Log($"Received malformed opcode: {opcode} (Ignoring)");
							return;
						}

						var handlePacket = HandlePacket[opcode];
						try
						{
							handlePacket.Read(packetReader);
						}
						catch (System.IO.EndOfStreamException e)
						{
							Log($"Received malformed opcode: {opcode} {e.Message} (Ignoring)");
							return;
						}

						Log($"Received opcode: {opcode}");

						handlePacket.Handle(netEvent.Peer);

						packetReader.Dispose();
						break;
				}
			}
		}

		server.Flush();

		Log("Server is no longer running");
	}

	private void Send(ServerPacket gamePacket, Peer peer)
	{
		var packet = default(Packet);
		packet.Create(gamePacket.Data, gamePacket.PacketFlags);
		byte channelID = 0;
		peer.Send(channelID, ref packet);
	}

	private Peer[] JoinPeers(Peer peer, Peer[] peers)
	{
		Peer[] thePeers;
		if (peer.Equals(default(Peer)))
		{
			thePeers = peers;
		}
		else
		{
			thePeers = new Peer[1 + peers.Length];
			thePeers[0] = peer;
			for (int i = 0; i < peers.Length; i++)
				thePeers[i + 1] = peers[i];
		}

		return thePeers;
	}

	protected override void DisconnectCleanup()
	{
		CTS.Cancel();
	}

	public override void Log(object message, ConsoleColor color = ConsoleColor.Green) => 
		Logger.Log($"[Server] {message}", color);
}

public class ENetServerCmd
{
	public ENetServerOpcode Opcode { get; set; }
	public object Data { get; set; }

	public ENetServerCmd(ENetServerOpcode opcode, object data = null)
	{
		Opcode = opcode;
		Data = data;
	}
}

public enum ENetServerOpcode
{
	Stop
}
