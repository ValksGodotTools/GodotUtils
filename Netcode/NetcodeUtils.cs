namespace GodotUtils;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using GodotUtils.Netcode;

public static class NetcodeUtils
{
    public static Dictionary<Type, PacketInfo<T>> MapPackets<T>()
    {
        var types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => typeof(T).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .OrderBy(x => x.Name)
            .ToList();

        var dict = new Dictionary<Type, PacketInfo<T>>();

        for (byte i = 0; i < types.Count; i++)
            dict.Add(types[i], new PacketInfo<T>
            {
                Opcode = i,
                Instance = (T)Activator.CreateInstance(types[i])
            });

        return dict;
    }
}

public class PacketInfo<T>
{
    public byte Opcode { get; set; }
    public T Instance { get; set; }
}
