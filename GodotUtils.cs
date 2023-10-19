namespace GodotUtils;

public class GU
{
    public static ServiceProvider Services { get; private set; }

    // Must be called externally from Game repository
    public static void Init(ServiceProvider services)
    {
        Services = services;
        Services.Add<Logger>();
    }
}
