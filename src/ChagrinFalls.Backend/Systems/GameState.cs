namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Holds the overall mutable state of the game, including the player's journal and inventory.
/// Passed to condition evaluators so they can query what the player knows and carries.
/// </summary>
public class GameState
{
    /// <summary>
    /// The player's journal tracking information they have learned.
    /// </summary>
    public PlayerJournal Journal { get; } = new();

    /// <summary>
    /// The player's inventory tracking items they have collected.
    /// </summary>
    public PlayerInventory Inventory { get; } = new();
}
