namespace GodotUtils.TopDown;

public static class PlayerUtils
{
	public static Vector2 GetMovementInputRaw(string prefix = "")
	{
		if (!string.IsNullOrWhiteSpace(prefix))
			prefix += "_";

		// Returns 0, -1 or 1 depending on input
		var inputHorz = Input.GetActionRawStrength($"{prefix}move_right") - Input.GetActionRawStrength($"{prefix}move_left");
		var inputVert = Input.GetActionRawStrength($"{prefix}move_down") - Input.GetActionRawStrength($"{prefix}move_up");

		// Normalize vector to prevent fast diagonal strafing
		return new Vector2(inputHorz, inputVert).Normalized();
	}

	public static Vector2 GetMovementInput(string prefix = "")
	{
		if (!string.IsNullOrWhiteSpace(prefix))
			prefix += "_";

		// GetActionStrength(...) supports controller sensitivity
		var inputHorz = Input.GetActionStrength($"{prefix}move_right") - Input.GetActionStrength($"{prefix}move_left");
		var inputVert = Input.GetActionStrength($"{prefix}move_down") - Input.GetActionStrength($"{prefix}move_up");

		// Normalize vector to prevent fast diagonal strafing
		return new Vector2(inputHorz, inputVert).Normalized();
	}
}
