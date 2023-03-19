namespace GodotUtils;

public partial class UIConsoleElement : PanelContainer
{
    public  object         Content             { get; }

    private PanelContainer CountPanelContainer { get; set; }
    private Label          CountLabel          { get; set; }
    private int            Count               { get; set; } = 1;
    private bool           IsCode              { get; }
    private ConsoleColor   Color               { get; }

    public UIConsoleElement(object content, ConsoleColor color = ConsoleColor.DarkGray, bool isCode = false)
    {
        Content = content;
        Color   = color;
        IsCode  = isCode;
    }

    public override void _Ready()
    {
        var marginContainer = new GMarginContainer(5, 5, 3, 3);
        var hbox = new HBoxContainer();
        var textEdit = new TextEdit();
        CountPanelContainer = new PanelContainer();
        var countMarginContainer = new GMarginContainer(5, 5, 0, 0);
        CountLabel = new Label();

        textEdit.Text = $"{Content}";
        textEdit.WrapMode = TextEdit.LineWrappingMode.Boundary;
        textEdit.ScrollFitContentHeight = true;
        textEdit.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        CountPanelContainer.Hide();
        CountPanelContainer.CustomMinimumSize = new Vector2(25, 0);
        CountPanelContainer.SizeFlagsVertical = SizeFlags.ShrinkCenter;
        CountPanelContainer.AddThemeStyleboxOverride("panel", new StyleBoxFlat
        {
            BgColor = new Color("ffa300af"),
            CornerRadiusBottomLeft = 15,
            CornerRadiusBottomRight = 15,
            CornerRadiusTopLeft = 15,
            CornerRadiusTopRight = 15
        });

        CountLabel.HorizontalAlignment = HorizontalAlignment.Center;
        CountLabel.VerticalAlignment = VerticalAlignment.Center;
        CountLabel.Text = $"{Count}";

        marginContainer.AddChild(hbox);
        hbox.AddChild(textEdit);
        hbox.AddChild(CountPanelContainer);
        CountPanelContainer.AddChild(countMarginContainer);
        countMarginContainer.AddChild(CountLabel);
        AddChild(marginContainer);

        // setup feed
        textEdit.Editable = false;
        textEdit.HighlightCurrentLine = false;
        textEdit.HighlightAllOccurrences = true;

        if (IsCode)
            textEdit.SyntaxHighlighter = new GCodeHighlighter();

        // This wrap mode does not wrap words that are larger than the consoles width
        textEdit.WrapMode = TextEdit.LineWrappingMode.Boundary;
        textEdit.SizeFlagsVertical = SizeFlags.ExpandFill;
        textEdit.AddThemeColorOverride("background_color", new Color(0, 0, 0, 0)); // 0 = transparent
        
        var godotColor = Color.ConvertToGodotColor();

        textEdit.AddThemeColorOverride("font_color", godotColor);
        textEdit.AddThemeColorOverride("font_readonly_color", godotColor);
        textEdit.AddThemeColorOverride("font_selected_color", godotColor);
        //textEdit.AddThemeFontSizeOverride("font_size", 14);

        // the default has transparency making the font hard to see, so we want to remove that
        //var grayness = 0.9f;
        //textEdit.AddThemeColorOverride("font_readonly_color", new Color(grayness, grayness, grayness, 1));

        textEdit.AddThemeStyleboxOverride("normal", new StyleBoxEmpty());
        textEdit.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
        textEdit.AddThemeStyleboxOverride("read_only", new StyleBoxEmpty());
    }

    public void ShowCount() => CountPanelContainer.Show();
    public void IncrementCount() => CountLabel.Text = $"{++Count}";
}
