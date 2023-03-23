namespace GodotUtils;

public partial class UILabeledCheckbox : UILabeled
{
    public Action<bool> ValueChanged { get; set; }

    private LabeledCheckboxOptions Options { get; set; }

    public UILabeledCheckbox(LabeledCheckboxOptions options) : base(options)
    {
        Options = options;
    }

    public override void CreateUI(HBoxContainer hbox)
    {
        var checkbox = Options.CheckBox;

        checkbox.Toggled += value => ValueChanged?.Invoke(value);

        hbox.AddChild(checkbox);
    }
}

public class LabeledCheckboxOptions : LabeledOptions
{
    public CheckBox CheckBox { get; set; } = new CheckBox();
}