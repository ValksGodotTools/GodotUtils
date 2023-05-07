namespace GodotUtils;

public static class ExtensionsControl
{
    public static void CoverEntireScreen(this Control control) =>
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

    public static void CenterToScreen(this Control control) =>
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.Center);
}
