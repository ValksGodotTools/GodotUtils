using ENet;

namespace GodotUtils.Netcode.Server;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public class ENetServer<TClientPacketOpcode> : ENetLow
{
	private static Dictionary<TClientPacketOpcode, APacketClient> HandlePacket { get; set; } = NetcodeUtils.LoadInstances<TClientPacketOpcode, APacketClient>("CPacket");
	private ConcurrentQueue<ServerPacket> Outgoing { get; } = new();

	public async void Start(ushort port, int maxClients)
	{
		CTS = new CancellationTokenSource();
		using var task = Task.Run(() => WorkerThread(port, maxClients), CTS.Token);
		await task;
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
						Log("Client disconnected - ID: " + netEvent.Peer.ID);
						break;

					case EventType.Timeout:
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

	public void Log(object message, ConsoleColor color = ConsoleColor.Green) => 
		Logger.Log($"[Server] {message}", color);
}
