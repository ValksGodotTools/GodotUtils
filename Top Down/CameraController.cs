namespace GodotUtils.TopDown;

public partial class CameraController : Node
{
	// Inspector
	[Export] 
	private float Speed { get; set; } = 100;

	[ExportGroup("Zoom")]
	[Export(PropertyHint.Range, "0.02, 0.16")] 
	private float ZoomIncrementDefault { get; set; } = 0.02f;

	[Export(PropertyHint.Range, "0.01, 1")] 
	private float MinZoom { get; set; } = 0.01f;

	[Export(PropertyHint.Range, "0.1, 10")] 
	private float MaxZoom { get; set; } = 1.0f;

	[Export(PropertyHint.Range, "0.01, 1")] 
	private float SmoothFactor { get; set; } = 0.25f;

	private float ZoomIncrement { get; set; } = 0.02f;
	private float TargetZoom { get; set; }

	// Panning
	private Vector2 InitialPanPosition { get; set; }
	private bool Panning { get; set; }
	private Camera2D Camera { get; set; }

	public override void _Ready()
	{
		Camera = GetParent<Camera2D>();

		// Set the initial target zoom value on game start
		TargetZoom = Camera.Zoom.X;
	}

	public override void _Process(double delta)
	{
		// Not sure if the below code should be in _PhysicsProcess or _Process

		// Arrow keys and WASD move camera around
		var dir = Vector2.Zero;

		if (GInput.IsMovingLeft())
			dir.X -= 1;

		if (GInput.IsMovingRight())
			dir.X += 1;

		if (GInput.IsMovingUp())
			dir.Y -= 1;

		if (GInput.IsMovingDown())
			dir.Y += 1;
		
		if (Panning)
			Camera.Position = InitialPanPosition - (GetViewport().GetMousePosition() / Camera.Zoom.X);

		// Arrow keys and WASD movement are added onto the panning position changes
		Camera.Position += dir.Normalized() * Speed;
	}

	public override void _PhysicsProcess(double delta)
	{
		// Prevent zoom from becoming too fast when zooming out
		ZoomIncrement = ZoomIncrementDefault * Camera.Zoom.X;

		// Lerp to the target zoom for a smooth effect
		Camera.Zoom = Camera.Zoom.Lerp(new Vector2(TargetZoom, TargetZoom), SmoothFactor);
	}

	// Not sure if this should be done in _Input or _UnhandledInput
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton)
			InputEventMouseButton(mouseButton);
	}

	private void InputEventMouseButton(InputEventMouseButton @event)
	{
		HandlePan(@event);
		HandleZoom(@event);
	}

	private void HandlePan(InputEventMouseButton @event)
	{
		// Left click to start panning the camera
		if (@event.ButtonIndex != MouseButton.Left)
			return;
		
		// Is this the start of a left click or is this releasing a left click?
		if (@event.IsPressed())
		{
			// Save the intial position
			InitialPanPosition = Camera.Position + (GetViewport().GetMousePosition() / Camera.Zoom.X);
			Panning = true;
		}
		else
			// Only stop panning once left click has been released
			Panning = false;
	}

	private void HandleZoom(InputEventMouseButton @event)
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
