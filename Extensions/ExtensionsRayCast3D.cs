namespace GodotUtils;

using Godot;

public static class ExtensionsRayCast3D
{
    public static void ExcludeRaycastParents(this RayCast3D raycast) =>
        ExcludeParents(raycast, raycast.GetParent());

    static void ExcludeParents(RayCast3D raycast, Node parent)
    {
        if (parent != null)
        {
            if (parent is CollisionObject3D collision)
                raycast.AddException(collision);

            ExcludeParents(raycast, parent.GetParentOrNull<Node>());
        }
    }
}
