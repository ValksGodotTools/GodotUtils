namespace GodotUtils;

public partial class UILabeledSlider : UILabeled
{
	public event Action<double> ValueChanged;

	private UILabeledSliderOptions Options { get; set; }

	public UILabeledSlider(UILabeledSliderOptions options)
	{
		Options = options;
	}

	public override void _Ready()
	{
		CreateUI();
	}

	private void CreateUI()
	{
		SetMargin(Options);

		SizeFlagsHorizontal = SizeFlags.ShrinkBegin;

		var hbox = new HBoxContainer();

		var label = new Label
		{
			Text = Options.Name,
			HorizontalAlignment = HorizontalAlignment.Left,
			VerticalAlignment = VerticalAlignment.Center,
			CustomMinimumSize = new Vector2(Options.MinLabelSize, 0)
		};

		var hslider = new HSlider
		{
			Value = Options.InitialValue,
			MaxValue = Options.MaxValue,
			Step = Options.Step,
			CustomMinimumSize = new Vector2(Options.MinElementSize, 0),
			SizeFlagsVertical = SizeFlags.ShrinkCenter
		};

		var lineEdit = new LineEdit
		{
			Editable = false,
			Alignment = HorizontalAlignment.Center,
			Text = Options.InitialValue + ""
		};

		hslider.ValueChanged += v =>
		{
			lineEdit.Text = v + "";
			ValueChanged?.Invoke(v);
		};

		hbox.AddChild(label);
		hbox.AddChild(hslider);
		hbox.AddChild(lineEdit);

		AddChild(hbox);
	}
}

public class UILabeledSliderOptions : UILabeledOptions
{
	public double InitialValue { get; set; }
	public double MaxValue     { get; set; } = 100;
	public double Step         { get; set; } = 1;
}

public class UILabeledOptions
{
	public string Name           { get; set; } = "Placeholder";
	public float  MinLabelSize   { get; set; } = 200;
	public float  MinElementSize { get; set; } = 200;
	public int    MarginLeft     { get; set; } = 10;
	public int    MarginRight    { get; set; } = 5;
	public int    MarginTop      { get; set; } = 5;
	public int    MarginBottom   { get; set; } = 5;
}
