using Newtonsoft.Json.Linq;

namespace GodotUtils;

public partial class UILabeledSlider : UILabeled
{
	public Action<double> ValueChanged { get; set; }

	private LabeledSliderOptions Options { get; set; }

	public UILabeledSlider(LabeledSliderOptions options) : base(options)
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
			ValueChanged?.Invoke(v);
		};

		hbox.AddChild(hslider);
		hbox.AddChild(lineEdit);
	}
}

public class LabeledSliderOptions : LabeledOptions
{
	public HSlider HSlider { get; set; } = new HSlider();
}
