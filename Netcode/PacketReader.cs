using System.IO;
using System.Reflection;

namespace GodotUtils.Netcode;

public class PacketReader : IDisposable
{
    private MemoryStream Stream { get; }
    private BinaryReader Reader { get; }
    private byte[] ReadBuffer { get; } = new byte[GamePacket.MaxSize];

    public PacketReader(Packet packet)
    {
        Stream = new MemoryStream(ReadBuffer);
        Reader = new BinaryReader(Stream);
        packet.CopyTo(ReadBuffer);
        packet.Dispose();
    }

    public byte ReadByte() => Reader.ReadByte();
    public sbyte ReadSByte() => Reader.ReadSByte();
    public char ReadChar() => Reader.ReadChar();
    public string ReadString() => Reader.ReadString();
    public bool ReadBool() => Reader.ReadBoolean();
    public short ReadShort() => Reader.ReadInt16();
    public ushort ReadUShort() => Reader.ReadUInt16();
    public int ReadInt() => Reader.ReadInt32();
    public uint ReadUInt() => Reader.ReadUInt32();
    public float ReadFloat() => Reader.ReadSingle();
    public double ReadDouble() => Reader.ReadDouble();
    public long ReadLong() => Reader.ReadInt64();
    public ulong ReadULong() => Reader.ReadUInt64();
    public byte[] ReadBytes(int count) => Reader.ReadBytes(count);
    public byte[] ReadBytes() => ReadBytes(ReadInt());

    public Vector2 ReadVector2() =>
        new(ReadFloat(), ReadFloat());

    public dynamic Read(Type t)
    {
        if (t == typeof(byte)) return ReadByte();
        if (t == typeof(sbyte)) return ReadSByte();
        if (t == typeof(char)) return ReadChar();
        if (t == typeof(string)) return ReadString();
        if (t == typeof(bool)) return ReadBool();
        if (t == typeof(short)) return ReadShort();
        if (t == typeof(ushort)) return ReadUShort();
        if (t == typeof(int)) return ReadInt();
        if (t == typeof(uint)) return ReadUInt();
        if (t == typeof(float)) return ReadFloat();
        if (t == typeof(double)) return ReadDouble();
        if (t == typeof(long)) return ReadLong();
        if (t == typeof(ulong)) return ReadULong();
        if (t == typeof(byte[])) return ReadBytes();
        if (t == typeof(Vector2)) return ReadVector2();

        if (t.IsGenericType)
        {
            var g = t.GetGenericTypeDefinition();

            if (g == typeof(IList<>) || g == typeof(List<>))
            {
                var vt = t.GetGenericArguments()[0];

                var count = ReadInt();

                dynamic list = Activator
                    .CreateInstance(typeof(List<>)
                    .MakeGenericType(vt));

                for (var i = 0; i < count; i++)
                    list.Add(Read(vt));

                return list;
            }

            if (g == typeof(IDictionary<,>) || g == typeof(Dictionary<,>))
            {
                var kt = t.GetGenericArguments()[0];
                var vt = t.GetGenericArguments()[1];

                var count = ReadInt();

                dynamic dict = Activator
                    .CreateInstance(typeof(Dictionary<,>)
                    .MakeGenericType(kt, vt));

                for (var i = 0; i < count; i++)
                    dict.Add(Read(kt), Read(vt));

                return dict;
            }
        }

        if (t.IsEnum)
        {
            var v = ReadByte();
            return Enum.ToObject(t, v);
        }

        if (t.IsValueType)
        {
            var v = Activator.CreateInstance(t);

            var fields = t
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(field => field.MetadataToken);

            foreach (var f in fields)
                f.SetValue(v, Read(f.FieldType));

            return v;
        }

        throw new NotImplementedException("PacketReader: " + t + " is not a supported type.");
    }

    public T Read<T>() =>
        Read(typeof(T));

    public void Dispose()
    {
        Stream.Dispose();
        Reader.Dispose();
    }
}
