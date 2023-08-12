namespace GodotUtils.Deprecated;

using Godot;
using System;

public partial class UIColorPickerBtn : UIElement
{
    public event Action<Color> ValueChanged;

    ColorPickerBtnOptions options;
    Color color;

    public UIColorPickerBtn(ColorPickerBtnOptions options) : base(options)
    {
        this.options = options;
    }

    public override void CreateUI(HBoxContainer hbox)
    {
        var colorPicker = options.ColorPickerButton;
        colorPicker.CustomMinimumSize = new Vector2(100, 0);

        colorPicker.ColorChanged += color =>
        {
            this.color = color;

            if (!options.OnlyUpdateOnPopupClosed)
                ValueChanged?.Invoke(color);
        };

        colorPicker.PopupClosed += () =>
        {
            if (options.OnlyUpdateOnPopupClosed)
                ValueChanged?.Invoke(color);
        };

        hbox.AddChild(colorPicker);
    }
}

public class ColorPickerBtnOptions : ElementOptions
{
    public ColorPickerButton ColorPickerButton       { get; set; } = new ColorPickerButton();
    public bool              OnlyUpdateOnPopupClosed { get; set; }
}
