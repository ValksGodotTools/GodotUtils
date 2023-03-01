namespace GodotUtils;

public static class ExtensionsContainer
{
	public static MarginContainer AddMargin(this MarginContainer container, int padding)
	{
		foreach (var margin in new string[] { "left", "right", "top", "bottom" })
			container.AddThemeConstantOverride($"margin_{margin}", padding);

		return container;
	}
}
