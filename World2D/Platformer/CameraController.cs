namespace GodotUtils.World2D.Platformer;

using Godot;

public partial class CameraController : Camera2D
{
    // Might convert these to [Export]s later thus they should stay as props
    float ZoomIncrement      { get; set; } = 0.08f;
    float MinZoom            { get; set; } = 1.5f;
    float MaxZoom            { get; set; } = 3.0f;
    float SmoothFactor       { get; set; } = 0.25f;
    float HorizontalPanSpeed { get; } = 8;

    float TargetZoom         { get; set; }

    public override void _Ready()
    {
        // Set the initial target zoom value on game start
        TargetZoom = base.Zoom.X;
    }

    public override void _PhysicsProcess(double delta)
    {
        var cameraWidth = GetViewportRect().Size.X / base.Zoom.X;
        var camLeftPos = Position.X - (cameraWidth / 2);
        var camRightPos = Position.X + (cameraWidth / 2);

        Panning(camLeftPos, camRightPos);
        Zooming();
        Boundaries(camLeftPos, camRightPos);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
            InputEventMouseButton(mouseEvent);
    }

    void Panning(float camLeftPos, float camRightPos)
    {
        if (Input.IsActionPressed("move_left"))
        {
            // Prevent the camera from going too far left
            if (camLeftPos > LimitLeft)
                Position -= new Vector2(HorizontalPanSpeed, 0);
        }

        if (Input.IsActionPressed("move_right"))
        {
            // Prevent the camera from going too far right
            if (camRightPos < LimitRight)
                Position += new Vector2(HorizontalPanSpeed, 0);
        }
    }

    void Zooming()
    {
        // Lerp to the target zoom for a smooth effect
        Zoom = Zoom.Lerp(new Vector2(TargetZoom, TargetZoom), SmoothFactor);
    }

    void Boundaries(float camLeftPos, float camRightPos)
    {
        if (camLeftPos < LimitLeft)
        {
            // Travelled this many pixels too far
            var gapDifference = Mathf.Abs(camLeftPos - LimitLeft);

            // Correct position
            Position += new Vector2(gapDifference, 0);
        }

        if (camRightPos > LimitRight)
        {
            // Travelled this many pixels too far
            var gapDifference = Mathf.Abs(camRightPos - LimitRight);

            // Correct position
            Position -= new Vector2(gapDifference, 0);
        }
    }

    void InputEventMouseButton(InputEventMouseButton @event)
    {
        InputZoom(@event);
    }

    void InputZoom(InputEventMouseButton @event)
    {
        // Not sure why or if this is required
        if (!@event.IsPressed())
            return;

        // Zoom in
        if (@event.ButtonIndex == MouseButton.WheelUp)
            TargetZoom += ZoomIncrement;

        // Zoom out
        if (@event.ButtonIndex == MouseButton.WheelDown)
            TargetZoom -= ZoomIncrement;

        // Clamp the zoom
        TargetZoom = Mathf.Clamp(TargetZoom, MinZoom, MaxZoom);
    }
}
