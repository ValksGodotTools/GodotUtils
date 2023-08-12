namespace GodotUtils.Deprecated;

using Godot;
using System;

public partial class UIOptionButton : UIElement
{
    public Action<long> ValueChanged { get; set; }

    public OptionButton OptionButton { get; set; }
    
    OptionButtonOptions Options { get; set; }

    public UIOptionButton(OptionButtonOptions options) : 
        base(options)
    {
        Options = options;
    }

    public override void CreateUI(HBoxContainer hbox)
    {
        OptionButton = Options.OptionButton;
        OptionButton.SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
        OptionButton.CustomMinimumSize = new Vector2(Options.MinElementSize, 0);

        foreach (var item in Options.Items)
            OptionButton.AddItem(item.AddSpaceBeforeEachCapital());

        OptionButton.ItemSelected += item => ValueChanged?.Invoke(item);

        hbox.AddChild(OptionButton);
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
