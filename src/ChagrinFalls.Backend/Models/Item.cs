namespace ChagrinFalls.Backend.Models;
/// <summary>
/// A named item defined at storybook level and referenced by
/// item <see cref="PointOfInterest"/>s in locations.
/// </summary>
public class Item
{
    /// <summary>Unique identifier used as the inventory item ID (e.g. "torn_letter").</summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>Display name shown to the player (e.g. "Torn Letter").</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Optional short description used in the editor.</summary>
    public string Description { get; set; } = string.Empty;
}
