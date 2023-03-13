namespace GodotUtils.UI;

public partial class UIConsole : Node
{
	private static TextEdit TextEdit { get; set; }
	private static LineEdit LineEdit { get; set; }

	public override void _Ready()
	{
		CreateUI();
	}

	public static void Test()
	{
		GD.Print("Test");
	}

	public static void AddMessage(object message)
	{
		// add the message to the console
		TextEdit.Text += $"{message}\n";

		// clear the line edit input
		LineEdit.Text = "";
	}

	private void CreateUI()
	{
		var vbox = new VBoxContainer();
		TextEdit = new TextEdit();
		LineEdit = new LineEdit();

		vbox.SetAnchorsPreset(Control.LayoutPreset.FullRect);
		vbox.AddThemeConstantOverride("separation", 0);
		TextEdit.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
		LineEdit.TextSubmitted += text => AddMessage(text);

		vbox.AddChild(TextEdit);
		vbox.AddChild(LineEdit);

		AddChild(vbox);
	}
}
