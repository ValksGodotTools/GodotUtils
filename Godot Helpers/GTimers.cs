namespace GodotUtils;

public class GTimers
{
	private Node Node { get; set; }

	public GTimers(Node node) => Node = node;

	public GTimer CreateTimer(int delayMS) =>
		new GTimer(Node, delayMS);

	public GTimer CreateTimer(Action action, int delayMS) =>
		new GTimer(Node, action, delayMS);
}

