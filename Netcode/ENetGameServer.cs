namespace GodotUtils.Netcode.Server;

public class ENetGameServer<TPlayerData> : ENetServer
{
    public Dictionary<uint, TPlayerData> Players { get; set; } = new();

    /// <summary>
    /// Get all players except for one player
    /// </summary>
    /// <param name="id">The player id to exclude</param>
    public Dictionary<uint, TPlayerData> GetOtherPlayers(uint id) =>
        Players.Where(x => x.Key != id).ToDictionary(x => x.Key, x => x.Value);

    protected override void Update()
    {
        foreach (var player in Players)
        {
            UpdatePlayer(player);
        }
    }

    /// <summary>
    /// The player that is currently being updated in the player loop
    /// </summary>
    protected virtual void UpdatePlayer(KeyValuePair<uint, TPlayerData> player) { }

    protected override void Disconnected(Event netEvent)
    {
        Players.Remove(netEvent.Peer.ID);
    }
}
