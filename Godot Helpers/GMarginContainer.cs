namespace GodotUtils;

public partial class GMarginContainer : MarginContainer
{
	public GMarginContainer(int padding = 5) => SetMarginAll(5);

	public void SetMarginAll(int padding)
	{
		foreach (var margin in new string[] { "left", "right", "top", "bottom" })
			AddThemeConstantOverride($"margin_{margin}", padding);
	}

	public void SetMarginLeft(int padding) =>
		AddThemeConstantOverride("margin_left", padding);

	public void SetMarginRight(int padding) =>
		AddThemeConstantOverride("margin_right", padding);

	public void SetMarginTop(int padding) =>
		AddThemeConstantOverride("margin_top", padding);

	public void SetMarginBottom(int padding) =>
		AddThemeConstantOverride("margin_bottom", padding);
}
