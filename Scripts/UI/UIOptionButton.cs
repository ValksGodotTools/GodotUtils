namespace GodotUtils;

public partial class UIOptionButton : UIElement
{
    public Action<long> ValueChanged { get; set; }

    private OptionButtonOptions Options { get; set; }

    public UIOptionButton(OptionButtonOptions options) : 
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

public class OptionButtonOptions : ElementOptions
{
    public OptionButton OptionButton { get; set; } = new OptionButton();
    public string[]     Items        { get; set; }

    public OptionButtonOptions(params string[] items)
    {
        Items = items;
    }
}
