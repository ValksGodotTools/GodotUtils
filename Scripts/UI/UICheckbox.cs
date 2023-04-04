namespace GodotUtils;

public partial class UICheckbox : UIElement
{
    public Action<bool> ValueChanged { get; set; }

    private CheckboxOptions Options { get; set; }

    public UICheckbox(CheckboxOptions options) : base(options)
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

public class CheckboxOptions : ElementOptions
{
    public CheckBox CheckBox { get; set; } = new CheckBox();
}
