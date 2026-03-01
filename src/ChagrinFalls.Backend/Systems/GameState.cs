namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Holds the overall mutable state of the game, including the player's journal, inventory,
/// current day, and time of day.
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

    /// <summary>
    /// The current day tracker (Day 1, Day 2, etc.).
    /// </summary>
    public DayTracker DayTracker { get; } = new();

    /// <summary>
    /// The current time of day clock (hours and minutes).
    /// </summary>
    public GameClock Clock { get; } = new();
}
