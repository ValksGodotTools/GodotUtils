namespace GodotUtils;

public static class ExtensionsNode 
{
    public static void Reparent(this Node curParent, Node newParent, Node node)
    {
        // Remove node from current parent
        curParent.RemoveChild(node);

        // Add node to new parent
        newParent.AddChild(node);
    }

    /// <summary>
    /// Get all children assuming they all extend from TNode
    /// </summary>
    public static TNode[] GetChildren<TNode>(this Node parent) where TNode : Node
    {
        var children = parent.GetChildren();
        var arr = new TNode[children.Count];

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

    public static void QueueFreeChildren(this Node parentNode)
    {
        foreach (Node node in parentNode.GetChildren())
            node.QueueFree();
    }

    public static void RemoveAllGroups(this Node node)
    {
        var groups = node.GetGroups();
        for (int i = 0; i < groups.Count; i++)
            node.RemoveFromGroup(groups[i]);
    }
}
