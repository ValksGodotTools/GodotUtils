﻿using Godot;

namespace GodotUtils;

public static class GPrintUtils
{
    public static void Warning(object message)
    {
        GD.PrintRich($"[color=yellow]{message}[/color]");
        GD.PushWarning(message);
    }
}