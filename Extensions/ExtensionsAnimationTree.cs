namespace GodotUtils;

using Godot;

public static class ExtensionsAnimationTree
{
    public static void SetCondition(this AnimationTree tree, StringName condition, Variant value) =>
        tree.Set($"parameters/conditions/{condition}", value);

    public static void SetParam(this AnimationTree tree, StringName path, Variant value) =>
        tree.Set($"parameters/{path}", value);
}
