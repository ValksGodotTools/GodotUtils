namespace GodotUtils;

public abstract partial class UILabeled : GMarginContainer
{
	public void SetMargin(UILabeledOptions options)
	{
		SetMarginLeft(options.MarginLeft);
		SetMarginRight(options.MarginRight);
		SetMarginTop(options.MarginTop);
		SetMarginBottom(options.MarginBottom);
	}
}
