using ENet;

namespace GodotUtils.Netcode.Client;

// ENet API Reference: https://github.com/SoftwareGuy/ENet-CSharp/blob/master/DOCUMENTATION.md
public class ENetClient<TServerPacketOpcode> : ENetLow
{
	public bool IsConnected => Interlocked.Read(ref connected) == 1;
	private long connected;

	private static Dictionary<TServerPacketOpcode, APacketServer> HandlePacket { get; set; }
	private ConcurrentQueue<ClientPacket> Outgoing { get; } = new();

	public async void Start(string ip, ushort port)
	{
		HandlePacket = NetcodeUtils.LoadInstances<TServerPacketOpcode, APacketServer>("SPacket");
		CTS = new CancellationTokenSource();
		using var task = Task.Run(() => WorkerThread(ip, port), CTS.Token);
		await task;
	}

	public void Send<TClientPacketOpcode>(TClientPacketOpcode opcode, APacket data = null, PacketFlags flags = PacketFlags.Reliable)
	{
		Outgoing.Enqueue(new ClientPacket(Convert.ToByte(opcode), flags, data));
	}

	private void WorkerThread(string ip, ushort port)
	{
		using var client = new Host();
		var address = new Address {
			Port = port
		};

		address.SetHost(ip);
		client.Create();

		var peer = client.Connect(address);

		/* 
		 * Pings are used both to monitor the liveness of the connection 
		 * and also to dynamically adjust the throttle during periods of 
		 * low traffic so that the throttle has reasonable responsiveness 
		 * during traffic spikes.
		 */
		uint pingInterval = 1000;

		// Will be ignored if maximum timeout is exceeded
		uint timeout = 5000;

		// The timeout for server not sending the packet to the client sent from the server
		uint timeoutMinimum = 5000;

		// The timeout for server not receiving the packet sent from the client
		uint timeoutMaximum = 5000;

		peer.PingInterval(pingInterval);
		peer.Timeout(timeout, timeoutMinimum, timeoutMaximum);

		while (!CTS.IsCancellationRequested)
		{
			var polled = false;

			// Outgoing
			while (Outgoing.TryDequeue(out var clientPacket))
			{
				byte channelID = 0; // The channel all networking traffic will be going through
				var packet = default(Packet);
				packet.Create(clientPacket.Data, clientPacket.PacketFlags);
				peer.Send(channelID, ref packet);
			}

			while (!polled)
			{
				if (client.CheckEvents(out Event netEvent) <= 0)
				{
					if (client.Service(15, out netEvent) <= 0)
						break;

					polled = true;
				}

				switch (netEvent.Type)
				{
					case EventType.None:
						break;

					case EventType.Connect:
						connected = 1;
						Log("Client connected to server");
						break;

					case EventType.Disconnect:
						DisconnectCleanup();
						Log("Client disconnected from server");
						break;

					case EventType.Timeout:
						DisconnectCleanup();
						Log("Client connection timeout");
						break;

					case EventType.Receive:
						var packet = netEvent.Packet;
						if (packet.Length > GamePacket.MaxSize)
						{
							Log($"Tried to read packet from server of size {packet.Length} when max packet size is {GamePacket.MaxSize}");
							packet.Dispose();
							continue;
						}

						var packetReader = new PacketReader(packet);
						var opcode = (TServerPacketOpcode)Enum.Parse(typeof(TServerPacketOpcode), packetReader.ReadByte().ToString(), true);
						var handlePacket = HandlePacket[opcode];
						handlePacket.Read(packetReader);

						handlePacket.Handle();

						packetReader.Dispose();

						break;
				}
			}
		}

		client.Flush();
	}

	protected override void DisconnectCleanup()
	{
		connected = 0;
		CTS.Cancel();
	}

	public override void Log(object message, ConsoleColor color = ConsoleColor.Cyan) => 
		Logger.Log($"[Client] {message}", color);
}
