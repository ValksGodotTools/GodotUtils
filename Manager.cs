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

public class Manager
{
    public static async Task Cleanup(Node node, ENetServer server, ENetClient client)
    {
        node.GetTree().AutoAcceptQuit = false;
        await CleanupNet(server, client);
        await CleanupLogger(node);
    }

    private static async Task CleanupNet(ENetServer server, ENetClient client)
    {
        if (ENetLow.ENetInitialized)
        {
            if (client != null)
            {
                client.Stop();

                while (client.IsRunning)
                    await Task.Delay(1);
            }

            if (server != null)
            {
                server.Stop();

                while (server.IsRunning)
                    await Task.Delay(1);
            }

            ENet.Library.Deinitialize();
        }
    }

    private static async Task CleanupLogger(Node node)
    {
        if (Logger.StillWorking())
            await Task.Delay(1);

        node.GetTree().Quit();
    }
}
