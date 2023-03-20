namespace GodotUtils;

public class PrevCurQueue<T>
{
    private List<T> Data = new();
    public double Progress { get; private set; }

    public T Previous { get; set; }
    public T Current { get; set; }

    public bool NotReady => Data.Count <= 1;

    private int Interval { get; set; }

    public PrevCurQueue(int interval)
    {
        Interval = interval;
        Current = default(T);
    }

    public void Add(T data)
    {
        Progress = 0; // reset progress as this is new incoming data
        Data.Add(data);

        if (Data.Count > 2) // only keep track of previous and current
            Data.RemoveAt(0);

        if (Data.Count == 1)
        {
            Previous = Data[0];
        }

        if (Data.Count == 2)
        {
            Previous = Data[0];
            Current = Data[1];
        }
    }

    public void UpdateProgress(double delta) => Progress += delta * (1000d / Interval); // reach value of 1.0 every 150ms
}
