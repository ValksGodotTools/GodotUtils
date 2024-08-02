namespace GodotUtils;

using Godot;
using System;

public static class ExtensionsNode
{
    /// <summary>
    /// Creates a looping tween that will stop and execute a callback when a condition is met.
    /// </summary>
    public static void TimerLoop(this Node node, double duration, Func<bool> condition, Action callback)
    {
        GTween tween = new(node);
        tween.Delay(duration);
        tween.Loop();
        tween.Callback(() =>
        {
            if (condition())
            {
                tween.Stop();
                callback();
            }
        });
    }

    /// <summary>
    /// Creates a delay followed by a callback only executed after the delay. The
    /// tween is attached to a node so when this node gets QueueFree'd the tween
    /// will get QueueFree'd as well.
    /// </summary>
    public static void TimerDelay(this Node node, double duration, Action callback)
    {
        GTween tween = new(node);
        tween.Delay(duration);
        tween.Callback(callback);
    }

    /// <summary>
    /// Asynchronously waits one procress frame. Remember any async operations will
    /// continue to run even after a node is QueueFree'd. If this is not desired have
    /// a look at ExtensionsNode.Delay(this Node node, double duration, Action callback)
    /// </summary>
    public async static Task WaitOneFrame(this Node parent) =>
        await parent.ToSignal(
            source: parent.GetTree(),
            signal: SceneTree.SignalName.ProcessFrame);

    /// <summary>
    /// Sets the PhysicsProcess and Process for all the first level children of a node.
    /// This is not a recursive operation.
    /// </summary>
    public static void SetChildrenEnabled(this Node node, bool enabled)
    {
        foreach (Node child in node.GetChildren())
        {
            child.SetProcess(enabled);
            child.SetPhysicsProcess(enabled);
        }
    }

    public static void AddChildDeferred(this Node node, Node child) =>
        node.CallDeferred(Godot.Node.MethodName.AddChild, child);

    /// <summary>
    /// Reparent a node to a new parent.
    /// </summary>
    public static void Reparent(this Node curParent, Node newParent, Node node)
    {
        // Remove node from current parent
        curParent.RemoveChild(node);

        // Add node to new parent
        newParent.AddChild(node);
    }

    /// <summary>
    /// Attempt to retrieve all children from parent of TNode type.
    /// </summary>
    public static TNode[] GetChildren<TNode>(this Node parent) where TNode : Node
    {
        Godot.Collections.Array<Node> children = parent.GetChildren();
        TNode[] arr = new TNode[children.Count];

        for (int i = 0; i < children.Count; i++)
            try
            {
                arr[i] = (TNode)children[i];
            }
            catch (InvalidCastException)
            {
                GD.PushError($"Could not get all children from parent " +
                    $"'{parent.Name}' because could not cast from " +
                    $"{children[i].GetType()} to {typeof(TNode)} for node " +
                    $"'{children[i].Name}'");
            }

        return arr;
    }

    /// <summary>
    /// QueueFree all the children attached to this node.
    /// </summary>
    public static void QueueFreeChildren(this Node parentNode)
    {
        foreach (Node node in parentNode.GetChildren())
            node.QueueFree();
    }

    /// <summary>
    /// Remove all groups this node is attached to.
    /// </summary>
    public static void RemoveAllGroups(this Node node)
    {
        Godot.Collections.Array<StringName> groups = node.GetGroups();
        for (int i = 0; i < groups.Count; i++)
            node.RemoveFromGroup(groups[i]);
    }
}
