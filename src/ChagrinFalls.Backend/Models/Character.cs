namespace ChagrinFalls.Backend.Models;
/// <summary>
/// A named character defined at storybook level and referenced by
/// character <see cref="PointOfInterest"/>s in locations.
/// </summary>
public class Character
{
    /// <summary>Unique identifier (e.g. "margery").</summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>Display name shown to the player (e.g. "Margery").</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Optional short description used in the editor.</summary>
    public string Description { get; set; } = string.Empty;
}
