namespace GodotUtils;

public abstract partial class UIElement : PanelContainer
{
    private ElementOptions Options { get; set; }
    private GMarginContainer MarginContainer { get; set; }

    public UIElement()
    {
        Options = new ElementOptions();
    }

    public UIElement(ElementOptions options)
    {
        Options = options;
    }

    public override void _Ready()
    {
        // Ex: TemperatureSlider or UsernameLineEdit
        Name = Options.Name + GetType().Name.Replace(nameof(UIElement), "");
        InitUI();
    }

    public abstract void CreateUI(HBoxContainer hbox);

    private void InitUI()
    {
        if (Options.HideBackPanel)
            AddThemeStyleboxOverride("panel", new StyleBoxEmpty());

        MarginContainer = new GMarginContainer();
        SetMargin(Options);

        var hbox = new HBoxContainer();

        var label = new GLabel(Options.Name)
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            CustomMinimumSize = new Vector2(Options.MinLabelSize, 0)
        };

        hbox.AddChild(label);

        CreateUI(hbox);

        MarginContainer.AddChild(hbox);

        AddChild(MarginContainer);
    }

    public void SetMargin(ElementOptions options)
    {
        MarginContainer.SetMarginLeft(options.MarginLeft);
        MarginContainer.SetMarginRight(options.MarginRight);
        MarginContainer.SetMarginTop(options.MarginTop);
        MarginContainer.SetMarginBottom(options.MarginBottom);
    }
}

public class ElementOptions
{
    public string Name           { get; set; } = "Placeholder";
    public float  MinLabelSize   { get; set; } = 200;
    public float  MinElementSize { get; set; } = 200;
    public int    MarginLeft     { get; set; } = 10;
    public int    MarginRight    { get; set; } = 5;
    public int    MarginTop      { get; set; } = 5;
    public int    MarginBottom   { get; set; } = 5;
    public bool   HideBackPanel  { get; set; } = true;
}
