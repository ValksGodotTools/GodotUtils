namespace GodotUtils.Deprecated;

using Godot;
using System;

// Even if it is useful, it looks like a eye sore.
public static class ExtensionsConsoleColor
{
    public static Color ConvertToGodotColor(this ConsoleColor color)
    {
        switch (color)
        {
            case ConsoleColor.Black:
                return Colors.Black;
            case ConsoleColor.DarkBlue:
                return Colors.DarkBlue;
            case ConsoleColor.DarkGreen:
                return Colors.DarkGreen;
            case ConsoleColor.DarkMagenta:
                return Colors.DarkMagenta;
            case ConsoleColor.DarkCyan:
                return Colors.DarkCyan;
            case ConsoleColor.DarkRed:
                return Colors.DarkRed;
            case ConsoleColor.DarkYellow:
                return Colors.Yellow;
            case ConsoleColor.DarkGray:
                return Colors.DarkGray;
            case ConsoleColor.Red:
                return Colors.Red;
            case ConsoleColor.Blue:
                return Colors.Blue;
            case ConsoleColor.Cyan:
                return Colors.Cyan;
            case ConsoleColor.Gray:
                return Colors.Gray;
            case ConsoleColor.Green:
                return Colors.Green;
            case ConsoleColor.Magenta:
                return Colors.Magenta;
            case ConsoleColor.White:
                return Colors.White;
            case ConsoleColor.Yellow:
                return Colors.Yellow;
            default:
                return Colors.White;
        }
    }
}
