namespace GodotUtils;

public static class ExtensionsRayCast
{
	/// <summary>
	/// A convience function to tell the raycast to exlude all parents that
	/// are of type CollisionObject2D (for example a ground raycast should
	/// only check for the ground, not the player itself)
	/// </summary>
	public static void ExcludeRaycastParents(this RayCast2D raycast, Node parent) 
	{
		if (parent != null)
		{
			if (parent is CollisionObject2D collision)
				raycast.AddException(collision);

			ExcludeRaycastParents(raycast, parent.GetParentOrNull<Node>());
		}
	}

	public static void ExcludeRaycastParents(this RayCast3D raycast) =>
		ExcludeParents(raycast, raycast.GetParent());

	private static void ExcludeParents(RayCast3D raycast, Node parent)
	{
		if (parent != null)
		{
			if (parent is CollisionObject3D collision)
				raycast.AddException(collision);

			ExcludeParents(raycast, parent.GetParentOrNull<Node>());
		}
	}

	/// <summary>
	/// Checks if any raycasts in a collection is colliding
	/// </summary>
	/// <param name="raycasts">Collection of raycasts to check</param>
	/// <returns>True if any ray cast is colliding, else false</returns>
	public static bool IsAnyRayCastColliding(this List<RayCast2D> raycasts)
	{
		foreach (var raycast in raycasts)
			if (raycast.IsColliding())
				return true;

		return false;
	}

	/// <summary>
	/// Returns the first raycasts in a collection which is colliding
	/// </summary>
	/// <param name="raycasts">Collection of raycasts to check</param>
	/// <returns>Raycast which is colliding, else default</returns>
	public static RayCast2D? GetAnyRayCastCollider(this List<RayCast2D> raycasts)
	{
		foreach (var raycast in raycasts)
			if (raycast.IsColliding())
				return raycast;

		return default;
	}
}
