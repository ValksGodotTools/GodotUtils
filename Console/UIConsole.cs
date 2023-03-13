using Godot;

namespace GodotUtils;

public partial class UIConsole : Control
{
	public  static bool             ScrollToBottom    { get; set; } = true;
							        			      
	private static bool             Initialized       { get; set; }
	private static TextEdit         Feed              { get; set; }
	private static LineEdit         Input             { get; set; }

	private Dictionary<int, string> InputHistory      { get; } = new();
	private int                     InputHistoryIndex { get; set; }
	private int                     InputHistoryNav   { get; set; }

	public static void AddMessage(object message)
	{
		// do not do anything if the console was not setup
		if (!Initialized)
			return;

		// add the message to the console
		Feed.Text += $"{message}\n";

		// clear the line edit input
		Input.Text = "";

		// scroll to bottom after adding the message
		ScrollDown();
	}

	public override void _Ready()
	{
		Initialized = true;
		CreateUI();
	}

	public override void _Input(InputEvent @event)
	{
		InputVisibility(@event);
		InputNavigateHistory();
	}

	private void InputVisibility(InputEvent @event)
	{
		if (@event is not InputEventKey inputEventKey)
			return;

		if (inputEventKey.IsKeyJustPressed(Key.F12))
		{
			ToggleVisibility();
			return;
		}
	}

	private void InputNavigateHistory()
	{
		if (!Visible || InputHistory.Count == 0)
			return;

		if (Godot.Input.IsActionJustPressed("ui_up"))
		{
			InputHistoryNav--;

			if (!InputHistory.ContainsKey(InputHistoryNav))
				InputHistoryNav++;

			Input.Text = InputHistory[InputHistoryNav];

			// if deferred is not use then something else will override these settings
			Input.CallDeferred("grab_focus");
			Input.CallDeferred("set", "caret_column", InputHistory[InputHistoryNav].Length);
		}

		if (Godot.Input.IsActionJustPressed("ui_down"))
		{
			InputHistoryNav++;

			if (!InputHistory.ContainsKey(InputHistoryNav))
				InputHistoryNav--;

			Input.Text = InputHistory[InputHistoryNav];

			// if deferred is not use then something else will override these settings
			Input.CallDeferred("grab_focus");
			Input.CallDeferred("set", "caret_column", InputHistory[InputHistoryNav].Length);
		}
	}

	private void ToggleVisibility()
	{
		Visible = !Visible;

		if (Visible)
		{
			Input.GrabFocus();
			ScrollDown();
		}
	}

	private static void ScrollDown()
	{
		if (ScrollToBottom)
			Feed.ScrollVertical = Mathf.Inf;
	}

	private void OnConsoleInputEntered(string text)
	{
		// case sensitivity and trailing spaces should not factor in here
		var inputToLowerTrimmed = text.Trim().ToLower();
		var inputArr = inputToLowerTrimmed.Split(' ');

		// extract command from input
		var cmd = inputArr[0];

		// do not do anything if cmd is just whitespace
		if (string.IsNullOrWhiteSpace(cmd))
			return;

		// keep track of input history
		InputHistory.Add(InputHistoryIndex++, inputToLowerTrimmed);
		InputHistoryNav = InputHistoryIndex;

		// check to see if the command is valid
		var command = Command.Instances.FirstOrDefault(x => x.IsMatch(cmd));

		if (command != null)
		{
			// extract cmd args from input
			var cmdArgs = inputArr.Skip(1).ToArray();

			// run the command
			command.Run(cmdArgs);
		}
		else
			// command does not exist
			Logger.Log($"The command '{cmd}' does not exist");

		// clear the input after the command is executed
		Input.Clear();
	}

	private void CreateUI()
	{
		// ensure the parent control is full rect
		SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

		var vbox = new VBoxContainer();
		Feed = new TextEdit();
		Input = new LineEdit();

		// setup vbox
		// as the parent is full rect, so does this vbox need to be
		vbox.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
		vbox.AddThemeConstantOverride("separation", 0);

		// setup feed
		Feed.Editable = false;
		Feed.HighlightCurrentLine = true;
		Feed.HighlightAllOccurrences = true;

		Feed.SyntaxHighlighter = new GCodeHighlighter();

		// This wrap mode does not wrap words that are larger than the consoles width
		Feed.WrapMode = TextEdit.LineWrappingMode.Boundary;
		Feed.SizeFlagsVertical = SizeFlags.ExpandFill;
		Feed.AddThemeColorOverride("background_color", new Color(0, 0, 0, 0.8f));
		Feed.AddThemeColorOverride("font_color", Colors.DarkGray);
		Feed.AddThemeFontSizeOverride("font_size", 14);

		// the default has transparency making the font hard to see, so we want to remove that
		Feed.AddThemeColorOverride("font_readonly_color", Colors.Black);

		// setup input
		Input.TextSubmitted += OnConsoleInputEntered;

		vbox.AddChild(Feed);
		vbox.AddChild(Input);

		AddChild(vbox);

		// initially hide the console
		Hide();
	}
}
