global using Godot;
global using GodotUtils.Netcode;
global using GodotUtils.Netcode.Server;
global using GodotUtils.Netcode.Client;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;

namespace GodotUtils;

// NuGet Package: https://www.nuget.org/packages/ValksGodotUtils/
// No longer being maintained or used but kept here just in case

public partial class Manager<TPlayerData> : Node
{
    private ENetGameServer<TPlayerData> Server { get; set; }
    private ENetGameClient Client { get; set; }

    protected void PreInit(ENetGameServer<TPlayerData> server, ENetGameClient client)
    {
        Server = server;
        Client = client;
    }

    public override async void _PhysicsProcess(double delta)
    {
        await Logger.Update();
    }

    public override async void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GetTree().AutoAcceptQuit = false;
            await Cleanup();
        }
    }

    private async Task Cleanup()
    {
        await CleanupNet();

        if (Logger.StillWorking())
            await Task.Delay(1);

        GetTree().Quit();
    }

    private async Task CleanupNet()
    {
        if (ENetLow.ENetInitialized)
        {
            if (Client != null)
            {
                Client.Stop();

                while (Client.IsRunning)
                    await Task.Delay(1);
            }

            if (Server != null)
            {
                Server.Stop();

                while (Server.IsRunning)
                    await Task.Delay(1);
            }

            ENet.Library.Deinitialize();
        }
    }
}
