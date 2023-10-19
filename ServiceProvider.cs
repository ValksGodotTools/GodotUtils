namespace GodotUtils;

// Solves the need for 'static'
public class ServiceProvider
{
    Dictionary<Type, object> services = new();

    public void AddService(object service) =>
        services.Add(service.GetType(), service);

    public T GetService<T>() => (T)services[typeof(T)];
}
