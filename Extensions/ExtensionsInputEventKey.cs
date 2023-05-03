namespace GodotUtils;

public static class ExtensionsInputEventKey
{
    public static bool IsKeyJustPressed(this InputEventKey v, Key key) =>
        v.Keycode == key && v.Pressed && !v.Echo;

    public static bool IsKeyJustReleased(this InputEventKey v, Key key) =>
        v.Keycode == key && !v.Pressed && !v.Echo;

    /// <summary>
    /// <para>Convert to a human readable key</para>
    /// <para>For example 'Ctrl + Shift + E'</para>
    /// </summary>
    public static string Readable(this InputEventKey v)
    {
        // If Keycode is not set than use PhysicalKeycode
        var keyWithModifiers = v.Keycode == Key.None ?
            v.GetPhysicalKeycodeWithModifiers() :
            v.GetKeycodeWithModifiers();

        return OS.GetKeycodeString(keyWithModifiers).Replace("+", " + ");
    }
}
