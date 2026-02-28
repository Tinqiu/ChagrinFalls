namespace ChagrinFalls.Backend.Models;

/// <summary>
/// Represents something a player can interact with at a <see cref="Location"/> —
/// either a character to talk to or an item to pick up.
/// </summary>
public class PointOfInterest
{
    /// <summary>
    /// Unique identifier for this point of interest within its location.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name shown to the player (e.g. "Margery", "Torn Letter").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Whether this is a character or an item.
    /// </summary>
    public PointOfInterestType Type { get; set; }

    /// <summary>
    /// For <see cref="PointOfInterestType.Character"/> POIs: the ID of the
    /// <see cref="ConversationEvent"/> to start when the player interacts.
    /// </summary>
    public string? ConversationEventId { get; set; }

    /// <summary>
    /// For <see cref="PointOfInterestType.Item"/> POIs: the item ID that will
    /// be added to the player's inventory when picked up.
    /// </summary>
    public string? ItemId { get; set; }
}

