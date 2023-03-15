namespace GodotUtils;

public partial class UILabeledOptionButton : UILabeled
{
    public Action<long> ValueChanged { get; set; }

    private LabeledOptionButtonOptions Options { get; set; }

    public UILabeledOptionButton(LabeledOptionButtonOptions options) : 
        base(options)
    {
        Options = options;
    }

    public override void CreateUI(HBoxContainer hbox)
    {
        var optionButton = Options.OptionButton;
        optionButton.SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
        optionButton.CustomMinimumSize = new Vector2(Options.MinElementSize, 0);

        foreach (var item in Options.Items)
            optionButton.AddItem(item.AddSpaceBeforeEachCapital());

        optionButton.ItemSelected += item => ValueChanged?.Invoke(item);

        hbox.AddChild(optionButton);
    }
}

public class LabeledOptionButtonOptions : LabeledOptions
{
    public OptionButton OptionButton { get; set; } = new OptionButton();
    public string[]     Items        { get; set; }

    public LabeledOptionButtonOptions(params string[] items)
    {
        Items = items;
    }
}
