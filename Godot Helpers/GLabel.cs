namespace GodotUtils;

public partial class GLabel : Label
{
    public GLabel(string text, int fontSize = 16)
    {
        Text = text;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        SetFontSize(fontSize);
    }

    public void SetFontSize(int v) => AddThemeFontSizeOverride("font_size", v);
}
