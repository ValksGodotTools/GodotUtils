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

public partial class Manager : Node
{
    public override void _PhysicsProcess(double delta)
    {
        Logger.Update();
    }

    public override async void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            GetTree().AutoAcceptQuit = false;
            await Cleanup();
        }
    }

    public async Task Cleanup()
    {
        await Net.Cleanup();

        if (Logger.StillWorking())
            await Task.Delay(1);

        GetTree().Quit();
    }
}
