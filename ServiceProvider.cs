namespace GodotUtils;

public class ServiceProvider
{
    Dictionary<Type, object> services = new();

    /// <summary>
    /// Add a Node that exists within the game tree. For example UIItemNotification
    /// exists within the game tree.
    /// </summary>
    public void Add(object service)
    {
        services.Add(service.GetType(), service);
    }

    /// <summary>
    /// Add a object that does not exist within the game tree. For example
    /// the Logger class does not extend from Node.
    /// </summary>
    public void Add<T>() where T : new()
    {
        T instance = new T();
        services.Add(instance.GetType(), instance);
    }

    public T Get<T>() => (T)services[typeof(T)];

    public override string ToString() => services.Print();
}
