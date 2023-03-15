namespace GodotUtils;

public static class ExtensionsInputEventKey
{
    public static bool IsKeyJustPressed(this InputEventKey inputEventKey, Key key) =>
        inputEventKey.Keycode == key && inputEventKey.Pressed && !inputEventKey.Echo;

    public static bool IsKeyJustReleased(this InputEventKey inputEventKey, Key key) =>
        inputEventKey.Keycode == key && !inputEventKey.Pressed && !inputEventKey.Echo;
}
