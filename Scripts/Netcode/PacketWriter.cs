namespace GodotUtils.Netcode;

using System.IO;
using System.Reflection;

public class PacketWriter : IDisposable
{
    public  MemoryStream Stream { get; } = new();
    private BinaryWriter Writer { get; }

    public PacketWriter()
    {
        Writer = new BinaryWriter(Stream);
    }

    public void Write(byte v) => Writer.Write(v);
    public void Write(sbyte v) => Writer.Write(v);
    public void Write(char v) => Writer.Write(v);
    public void Write(string v) => Writer.Write(v);
    public void Write(bool v) => Writer.Write(v);
    public void Write(short v) => Writer.Write(v);
    public void Write(ushort v) => Writer.Write(v);
    public void Write(int v) => Writer.Write(v);
    public void Write(uint v) => Writer.Write(v);
    public void Write(float v) => Writer.Write(v);
    public void Write(double v) => Writer.Write(v);
    public void Write(long v) => Writer.Write(v);
    public void Write(ulong v) => Writer.Write(v);

    public void Write(byte[] v, bool header = true)
    {
        if (header)
            Write(v.Length);

        Writer.Write(v);
    }

    public void Write(Vector2 v)
    {
        Writer.Write(v.X);
        Writer.Write(v.Y);
    }

    public void Write<T>(T v)
    {
        switch (v)
        {
            case byte k: Write(k); return;
            case sbyte k: Write(k); return;
            case char k: Write(k); return;
            case string k: Write(k); return;
            case bool k: Write(k); return;
            case short k: Write(k); return;
            case ushort k: Write(k); return;
            case int k: Write(k); return;
            case uint k: Write(k); return;
            case float k: Write(k); return;
            case double k: Write(k); return;
            case long k: Write(k); return;
            case ulong k: Write(k); return;
            case byte[] k: Write(k, true); return;
            case Vector2 k: Write(k); return;
        }

        var t = v.GetType();
        dynamic d = v;

        if (t.IsGenericType)
        {
            var g = t.GetGenericTypeDefinition();

            if (g == typeof(IList<>) || g == typeof(List<>))
            {
                Write(d.Count);

                foreach (var item in d)
                    Write<dynamic>(item);

                return;
            }

            if (g == typeof(IDictionary<,>) || g == typeof(Dictionary<,>))
            {
                Write(d.Count);

                foreach (var item in d)
                {
                    Write<dynamic>(item.Key);
                    Write<dynamic>(item.Value);
                }

                return;
            }
        }

        if (t.IsEnum)
        {
            Write((byte)d);
            return;
        }

        if (t.IsValueType)
        {
            var fields = t
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(field => field.MetadataToken);

            foreach (var field in fields)
                Write<dynamic>(field.GetValue(d));

            return;
        }

        throw new NotImplementedException("PacketWriter: " + t + " is not a supported type.");
    }

    public void WriteAll(params dynamic[] values)
    {
        foreach (var value in values)
            Write<dynamic>(value);
    }

    public void Dispose()
    {
        Stream.Dispose();
        Writer.Dispose();
    }
}
