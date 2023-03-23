namespace GodotUtils;

public partial class UIConsole : PanelContainer
{
    // exports
    [Export] private NodePath NodePathFeed           { get; set; }
    [Export] private NodePath NodePathInput          { get; set; }
    [Export] private NodePath NodePathSettingsButton { get; set; }
    [Export] private NodePath NodePathSettingsPopup  { get; set; }
    [Export] private NodePath NodePathSettingsVBox   { get; set; }

    // nodes
    private static TextEdit Feed               { get; set; }
    private LineEdit        Input              { get; set; }
    private Button          SettingsBtn        { get; set; }
    private PopupPanel      SettingsPopup      { get; set; }
    private CheckBox        SettingsAutoScroll { get; set; }

    // vars
    public static bool IsVisible { get => Instance.Visible; }

    private ConsoleHistory   History    { get; set; } = new();
    private static bool      AutoScroll { get; set; } = true;
    private static UIConsole Instance   { get; set; }

    public override void _Ready()
    {
        Instance = this;

        Feed          = GetNode<TextEdit>(NodePathFeed);
        Input         = GetNode<LineEdit>(NodePathInput);
        SettingsBtn   = GetNode<Button>(NodePathSettingsButton);
        SettingsPopup = GetNode<PopupPanel>(NodePathSettingsPopup);
        
        var settingsVBox = GetNode(NodePathSettingsVBox);
        SettingsAutoScroll = settingsVBox.GetNode<CheckBox>("AutoScroll");

        Input.TextSubmitted += OnConsoleInputEntered;
        SettingsBtn.Pressed += OnSettingsBtnPressed;
        SettingsAutoScroll.Toggled += v => AutoScroll = v;
        SettingsAutoScroll.ButtonPressed = AutoScroll;

        Hide();
    }

    public override async void _Input(InputEvent @event)
    {
        await InputVisibility(@event);
        InputNavigateHistory();
    }

    public static void AddMessage(object message)
    {
        // Prevent text feed from becoming too large
        if (Feed.Text.Count() > 1000)
            // If there are say 2353 characters then 2353 - 1000 = 1353 characters
            // which is how many characters we need to remove to get back down to
            // 1000 characters
            Feed.Text = Feed.Text.Remove(0, Feed.Text.Count() - 1000);
        
        // ISSUE: Feed scrolls to very top when a new message is added and AutoScroll is disabled
        Feed.Text += $"\n{message}";
        ScrollDown();
    }

    private static void ScrollDown()
    {
        if (AutoScroll)
            Feed.ScrollVertical = (int)Feed.GetVScrollBar().MaxValue;
    }

    private void OnSettingsBtnPressed()
    {
        if (!SettingsPopup.Visible)
            SettingsPopup.PopupCentered();
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

    private async Task InputVisibility(InputEvent @event)
    {
        if (@event is not InputEventKey inputEventKey)
            return;

        if (inputEventKey.IsKeyJustPressed(Key.F12))
        {
            await ToggleVisibility();
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

    private async Task ToggleVisibility()
    {
        Visible = !Visible;

        if (Visible)
        {
            Input.GrabFocus();

            // This isn't ideal as you can see the scroll spas out for 1 frame
            // But it is all I can think of doing right now
            // If no task delay is set here then scroll down will not register
            // at all
            await Task.Delay(1);

            ScrollDown();
        }
    }
}
