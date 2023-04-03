namespace GodotUtils;

public static class GWindow
{
    public static void SetTitle(string title) => DisplayServer.WindowSetTitle(title);
    
    public static int GetWidth() => DisplayServer.WindowGetSize().X;
    public static int GetHeight() => DisplayServer.WindowGetSize().Y;
}
