namespace GodotUtils;

public class GTimers
{
    private Node node;

    public GTimers(Node node) => this.node = node;

    public GTimer CreateTimer(int delayMS) => new GTimer(node, delayMS);

    public GTimer CreateTimer(Action action, int delayMS) =>
        new GTimer(node, action, delayMS);
}

