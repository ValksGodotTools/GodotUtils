namespace GodotUtils;

public partial class UIColorPickerBtn : UIElement
{
    public Action<Color> ValueChanged { get; set; }
    private ColorPickerBtnOptions Options { get; set; }
    private Color Color { get; set; }

    public UIColorPickerBtn(ColorPickerBtnOptions options) : base(options)
    {
        Options = options;
    }

    public override void CreateUI(HBoxContainer hbox)
    {
        var colorPicker = Options.ColorPickerButton;
        colorPicker.CustomMinimumSize = new Vector2(100, 0);

        colorPicker.ColorChanged += color =>
        {
            Color = color;

            if (!Options.OnlyUpdateOnPopupClosed)
                ValueChanged?.Invoke(color);
        };

        colorPicker.PopupClosed += () =>
        {
            if (Options.OnlyUpdateOnPopupClosed)
                ValueChanged?.Invoke(Color);
        };

        hbox.AddChild(colorPicker);
    }
}

public class ColorPickerBtnOptions : ElementOptions
{
    public ColorPickerButton ColorPickerButton       { get; set; } = new ColorPickerButton();
    public bool              OnlyUpdateOnPopupClosed { get; set; }
}
