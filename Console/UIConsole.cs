using Godot;

namespace GodotUtils;

/*
 * Attach this script to a Control node under the CanvasLayer node.
 * Make sure this UIConsole node is below every other node so the console
 * will appear on top of every node.
 */
public partial class UIConsole : Control
{
    private static bool             AutoScroll         { get; set; } = true;
    public  static bool             IsVisible          { get => Instance.Visible; }
                                                       
    private static bool             Initialized        { get; set; }
    private static LineEdit         Input              { get; set; }
    private        ConsoleHistory   History            { get; set; } = new();
    private static VBoxContainer    ScrollVBox         { get; set; }
    private static UIConsoleElement PrevConsoleElement { get; set; }
    private static ScrollContainer  ScrollContainer    { get; set; }
    private static UIConsole        Instance           { get; set; }

    public static async Task AddMessage(object message, ConsoleColor color, bool isCode = false)
    {
        // do not do anything if the console was not setup
        if (!Initialized)
            return;

        // add the message to the console
        var consoleElement = new UIConsoleElement(message, color);

        if (PrevConsoleElement?.Content.ToString() == message.ToString())
        {
            PrevConsoleElement.ShowCount();
            PrevConsoleElement.IncrementCount();
        }
        else
        {
            ScrollVBox.AddChild(consoleElement, isCode);
            PrevConsoleElement = consoleElement;
        }

        // scroll to bottom after adding the message
        await Task.Delay(1);
        ScrollDown();
    }

    public override void _Ready()
    {
        Instance = this;
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
        // If console is not visible or there is no history to navigate do nothing
        if (!Visible || History.NoHistory())
            return;

        if (Godot.Input.IsActionJustPressed("ui_up"))
        {
            var historyText = History.MoveUpOne();

            Input.Text = historyText;

            // if deferred is not use then something else will override these settings
            Input.CallDeferred("grab_focus");
            Input.CallDeferred("set", "caret_column", historyText.Length);
        }

        if (Godot.Input.IsActionJustPressed("ui_down"))
        {
            var historyText = History.MoveDownOne();

            Input.Text = historyText;

            // if deferred is not use then something else will override these settings
            Input.CallDeferred("grab_focus");
            Input.CallDeferred("set", "caret_column", historyText.Length);
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
        if (AutoScroll)
            ScrollContainer.ScrollVertical = (int)ScrollContainer.GetVScrollBar().MaxValue;
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
        History.Add(inputToLowerTrimmed);

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

        var panelContainer = new PanelContainer();
        panelContainer.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

        var vbox1 = new VBoxContainer();
        panelContainer.AddChild(vbox1);

        var margin = new GMarginContainer(10);
        margin.SizeFlagsVertical = SizeFlags.ExpandFill;

        Input = new LineEdit();
        Input.TextSubmitted += OnConsoleInputEntered;

        CreateTopHBoxButtons(vbox1);

        vbox1.AddChild(margin);
        vbox1.AddChild(Input);

        ScrollContainer = new ScrollContainer();
        ScrollContainer.VerticalScrollMode = ScrollContainer.ScrollMode.ShowNever;
        margin.AddChild(ScrollContainer);

        ScrollVBox = new VBoxContainer();
        ScrollVBox.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        //ScrollVBox.AddThemeConstantOverride("separation", 0);
        ScrollContainer.AddChild(ScrollVBox);

        AddChild(panelContainer);

        Hide();
    }

    private void CreateTopHBoxButtons(Node parent)
    {
        var marginContainer = new GMarginContainer(10, 10, 5, 5);
        var hbox = new HBoxContainer();
        
        marginContainer.AddChild(hbox);
        CreateAutoScrollUIButton(hbox);

        parent.AddChild(marginContainer);
    }

    private void CreateAutoScrollUIButton(Node parent)
    {
        var panel = new PanelContainer();
        var hbox = new HBoxContainer();
        var label = new Label();
        var checkBox = new CheckButton();

        label.Text = "Autoscroll";
        checkBox.ButtonPressed = AutoScroll;
        checkBox.Toggled += v => AutoScroll = v;

        panel.AddChild(hbox);
        hbox.AddChild(label);
        hbox.AddChild(checkBox);

        parent.AddChild(panel);
    }
}
