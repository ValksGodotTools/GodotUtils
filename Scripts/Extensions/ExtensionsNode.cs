namespace GodotUtils;

public static class ExtensionsNode 
{
    /// <summary>
    /// Get all children assuming they all extend from TNode
    /// </summary>
    public static TNode[] GetChildren<TNode>(this Node parent) where TNode : Node
    {
        var children = parent.GetChildren();
        var arr = new TNode[children.Count];

        for (int i = 0; i < children.Count; i++)
            arr[i] = (TNode)children[i];

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
