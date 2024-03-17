namespace GodotUtils;

using Godot;

public static class ExtensionsAnimationTree
{
    /// <summary>
    /// Set a condition to 'value' then flip 'value' when 0.1 seconds have passed.
    /// Conditions only need to be set for a short duration otherwise you will find
    /// yourself trying to set all the conditions you set earlier to false again.
    /// </summary>
    public static void SetCondition(this AnimationTree tree, StringName path, bool value)
    {
        tree.Set($"parameters/conditions/{path}", value);

        GTween tween = new GTween(tree);
        tween.Delay(0.1);
        tween.Callback(() => tree.Set($"parameters/conditions/{path}", !value));
    }

    public static void SetParam(this AnimationTree tree, StringName path, Variant value) =>
        tree.Set($"parameters/{path}", value);
}
