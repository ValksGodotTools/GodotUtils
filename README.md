## What is this?
A utils library for Godot 4 C# RC5+

## Multiplayer
Below is a quick and dirty example of how this could be used.
```cs
public partial class SandboxExample : Node
{
    public static ENetServer<ClientPacketOpcode> Server { get; set; } = new();
    public static ENetClient<ServerPacketOpcode> Client { get; set; } = new();

    public override async void _Ready()
    {
        Server.Start(25565, 100);
        Client.Start("localhost", 25565);

        while (!Client.IsConnected)
            await Task.Delay(1);
        
        Client.Send(ClientPacketOpcode.Info, new CPacketInfo {
            Username = "Freddy"
        });

        Server.Kick(0, DisconnectOpcode.Banned);
    }
}
```

```cs
public class CPacketInfo : APacketClient
{
	public string Username { get; set; }

	public override void Write(PacketWriter writer)
	{
		writer.Write(Username);
	}

	public override void Read(PacketReader reader)
	{
		Username = reader.ReadString();
	}

	public override void Handle(Peer peer)
	{
		Logger.Log("Hello from the server. The username is " + Username);
		Main.Server.Send(ServerPacketOpcode.Pong, new SPacketPong
		{
			Data = 66
		}, peer);
	}
}
```

```cs
public enum ServerPacketOpcode
{
	Pong,
	Info
}

public enum ClientPacketOpcode
{
	Info
}
```

## Other Features
There are many other things that are not documented here. Check out the source. More will be documented in time.

## Install
Add this as a submodule to your GitHub repo
```
git submodule add https://github.com/Valks-Games/GodotUtils GodotUtils
```

Make sure this is in your `.csproj`
```xml
<ItemGroup>
	<PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
</ItemGroup>
```
