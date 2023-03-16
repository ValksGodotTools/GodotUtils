## What is this?
A utils library for Godot 4 C# RC5+

## Multiplayer
Quick and dirty netcode example
```cs
public static class Net
{
    public static GameServer Server { get; set; } = new();
    public static GameClient Client { get; set; } = new();
}
```

```cs
public partial class Main : Node
{
    public override async void _Ready()
    {
        Net.Server.Start(25565, 100);
        Net.Client.Connect("localhost", 25565);

        while (!Net.Client.IsConnected)
            await Task.Delay(1);

        Net.Client.Send(new CPacketJoin { Username = "Fred" } );
    }

    public override void _PhysicsProcess(double delta)
    {
        Logger.Update();
        GodotCommands.Update();
    }

    public override async void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
            await Manager.Cleanup(this, Net.Server, Net.Client);
    }
}
```

```cs
public class CPacketJoin : APacketClient
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
        Net.Server.Log($"Player {peer.ID} joined");

        Net.Server.Players.Add(peer.ID, new PlayerData
        {
            Username = Username
        });

        Net.Server.Send(new SPacketSpawnPlayer { Id = peer.ID }, peer);
    }
}
```

```cs
public class SPacketSpawnPlayer : APacketServer
{
    public uint Id { get; set; }

    public override void Write(PacketWriter writer)
    {
        writer.Write((uint)Id);
    }

    public override void Read(PacketReader reader)
    {
        Id = reader.ReadUInt();
    }

    public override void Handle()
    {
        GameMaster.CreateOtherPlayer(Id, new PlayerData
        {
            Position = new Vector2(500, 500)
        });
    }
}
```

## UIConsole
https://user-images.githubusercontent.com/6277739/225775311-cb7bc7cf-c0ed-42c3-9ec9-d85622b6a41e.mp4

## Other Features
There is a lot of stuff in this library. I feel like if I document it here, it's just going to change later. So I'll leave it up to the reader to explore the source themselves.

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
