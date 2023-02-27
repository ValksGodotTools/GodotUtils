using System.Reflection;

namespace GodotUtils;

public static class ReflectionUtils
{
	/// <summary>
	/// Loads instances of each class into a dictionary sorted by strings
	/// </summary>
    public static Dictionary<TKey, TValue> LoadInstances<TKey, TValue>(string prefix) =>
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(TValue).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(Activator.CreateInstance).Cast<TValue>()
            .ToDictionary(x => (TKey)Enum.Parse(typeof(TKey), x.GetType().Name.Replace(prefix, "")), x => x);
}
