using GodotUtils.Netcode.Client;
using GodotUtils.Netcode.Server;

namespace GodotUtils.Netcode;

public static class Net
{
    public static bool ENetInitialized { get; set; }
    public static ENetServer Server { get; set; } = new();
    public static ENetClient Client { get; set; } = new();

    public static async Task Cleanup()
    {
        if (ENetInitialized)
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
