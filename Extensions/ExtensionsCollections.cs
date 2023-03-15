using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GodotUtils;

public static class CollectionExtensions
{
    /// <summary>
    /// Prints a collection in a readable format
    /// </summary>
    public static string Print<T>(this IEnumerable<T> value, bool newLine = true) =>
        value != null ? string.Join(newLine ? "\n" : ", ", value) : null;

    /// <summary>
    /// Prints the entire object in a readable format (supports Godot properties)
    /// If you should ever run into a problem, see the IgnorePropsResolver class to ignore more
    /// properties.
    /// </summary>
    public static string PrintFull(this object v) =>
        JsonConvert.SerializeObject(v, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = new IgnorePropsResolver() // ignore all Godot props
        });

    /// <summary>
    /// A convience method for a foreach loop at the the sacrafice of debugging support
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
    {
        foreach (var element in value)
            action(element);
    }

    /// <summary>
    /// Returns true if a dictionary has a duplicate key and warns the coder
    /// </summary>
    public static bool Duplicate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string path = null)
    {
        if (!dict.ContainsKey(key))
            return false;

        //Logger.LogWarning($"'{caller}' tried to add duplicate key '{key}' to dictionary\n" +
        //                    $"   at {path} line:{lineNumber}");

        return true;
    }

    /// <summary>
    /// Returns true if a dictionary has a non-existent key and warns the coder
    /// </summary>
    public static bool DoesNotHave<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string caller = null,
        [CallerFilePath] string path = null)
    {
        if (dict.ContainsKey(key))
            return false;

        //Logger.LogWarning($"'{caller}' tried to access non-existent key '{key}' from dictionary\n" +
        //                    $"   at {path} line:{lineNumber}");

        return true;
    }

    /// <summary>
    /// Used when doing JsonConvert.SerializeObject to ignore Godot properties
    /// as these are massive.
    /// </summary>
    private class IgnorePropsResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            // Ignored properties (prevents crashes)
            var ignoredProps = new Type[]
            {
                typeof(GodotObject),
                typeof(Node),
                typeof(NodePath)
            };

            foreach (var ignoredProp in ignoredProps)
            {
                if (ignoredProp.GetProperties().Contains(member))
                    prop.Ignored = true;

                if (prop.PropertyType == ignoredProp || prop.PropertyType.IsSubclassOf(ignoredProp))
                    prop.Ignored = true;
            }

            return prop;
        }
    }
}
