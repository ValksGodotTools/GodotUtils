namespace GodotUtils;

public partial class UISlider : UIElement
{
    public Action<float> ValueChanged { get; set; }

    private SliderOptions Options { get; set; }

    public UISlider(SliderOptions options) : base(options)
    {
        Options = options;
    }

    public override void CreateUI(HBoxContainer hbox)
    {
        SizeFlagsHorizontal = SizeFlags.ShrinkBegin;

        var hslider = Options.HSlider;
        hslider.CustomMinimumSize = new Vector2(Options.MinElementSize, 0);
        hslider.SizeFlagsVertical = SizeFlags.ShrinkCenter;

        var lineEdit = new LineEdit
        {
            Editable = false,
            Alignment = HorizontalAlignment.Center,
            Text = hslider.Value + ""
        };

        hslider.ValueChanged += v =>
        {
            lineEdit.Text = v + "";
            ValueChanged?.Invoke((float)v);
        };

        hbox.AddChild(hslider);
        hbox.AddChild(lineEdit);
    }
}

public class SliderOptions : ElementOptions
{
    public HSlider HSlider { get; set; } = new HSlider();
}
