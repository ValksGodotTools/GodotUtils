namespace GodotUtils.Deprecated;

using Godot;
using System;

public partial class UISlider : UIElement
{
    public Action<float> ValueChanged { get; set; }

    public HSlider Slider { get; set; }

    SliderOptions Options { get; set; }

    public UISlider(SliderOptions options) : base(options)
    {
        Options = options;
    }

    public override void CreateUI(HBoxContainer hbox)
    {
        SizeFlagsHorizontal = SizeFlags.ShrinkBegin;

        Slider = Options.HSlider;
        Slider.CustomMinimumSize = new Vector2(Options.MinElementSize, 0);
        Slider.SizeFlagsVertical = SizeFlags.ShrinkCenter;

        var lineEdit = new LineEdit
        {
            Editable = false,
            Alignment = HorizontalAlignment.Center,
            Text = Slider.Value + ""
        };

        Slider.ValueChanged += v =>
        {
            lineEdit.Text = v + "";
            ValueChanged?.Invoke((float)v);
        };

        hbox.AddChild(Slider);
        hbox.AddChild(lineEdit);
    }
}

public class SliderOptions : ElementOptions
{
    public HSlider HSlider { get; set; } = new HSlider();
}
