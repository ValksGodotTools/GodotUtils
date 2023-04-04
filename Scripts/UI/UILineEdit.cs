namespace GodotUtils;

public partial class UILineEdit : UIElement
{
    public Action<string> ValueChanged { get; set; }

    private LineEditOptions Options { get; set; }
    private string PrevText { get; set; } = "";

    public UILineEdit(LineEditOptions options) : base(options)
    {
        Options = options;
    }

    public override void CreateUI(HBoxContainer hbox)
    {
        var lineEdit = Options.LineEdit;
        lineEdit.CustomMinimumSize = new Vector2(Options.MinElementSize, 0);

        lineEdit.TextChanged += text =>
        {
            if (Options.Trimmed)
                text = text.Trim();

            if (PrevText == text)
                return;

            PrevText = text;

            if (Options.IgnoreEmpty)
            {
                if (!string.IsNullOrWhiteSpace(text))
                    ValueChanged?.Invoke(text);
            }
            else
            {
                ValueChanged?.Invoke(text);
            }
        };

        hbox.AddChild(lineEdit);
    }
}

public class LineEditOptions : ElementOptions
{
    public bool     IgnoreEmpty { get; set; } = true;
    public bool     Trimmed     { get; set; } = true;
    public LineEdit LineEdit    { get; set; } = new LineEdit();
}
