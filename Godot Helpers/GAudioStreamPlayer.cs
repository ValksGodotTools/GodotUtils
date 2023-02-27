namespace GodotUtils;

public class GAudioStreamPlayer 
{
    private SFXSound AudioStreamPlayer { get; }

    /// <summary>
    /// The volume ranging from a value of 0 to 100
    /// </summary>
    public float Volume 
    {
        get { return AudioStreamPlayer.VolumeDb.Remap(-40, 0, 0, 100); }
        set 
        {
            var v = value.Remap(0, 100, -40, 0);

            if (value == 0)
                v = 0;

            AudioStreamPlayer.VolumeDb = v; 
        }
    }

    /// <summary>
    /// The pitch value ranging from 0.01 to 4.00
    /// </summary>
    public float Pitch 
    {
        get { return AudioStreamPlayer.PitchScale; }
        set { AudioStreamPlayer.PitchScale = value; }
    }

    public AudioStream Stream 
    {
        get { return AudioStreamPlayer.Stream; }
        set { AudioStreamPlayer.Stream = value; }
    }

    public GAudioStreamPlayer(Node target, bool deleteOnFinished = false) 
    {
        AudioStreamPlayer = new();
        target.AddChild(AudioStreamPlayer);

		if (deleteOnFinished)
			AudioStreamPlayer.Connect("finished", new Callable(AudioStreamPlayer, "OnFinished"));
    }

    public void Play() => AudioStreamPlayer.Play();
    public void Stop() => AudioStreamPlayer.Stop();
	public void QueueFree() => AudioStreamPlayer.QueueFree();
}

public partial class SFXSound : AudioStreamPlayer
{
	private void OnFinished() 
	{
		QueueFree();	
	}
}
