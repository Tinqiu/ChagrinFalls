namespace ChagrinFalls.Backend.Models;

/// <summary>
/// Represents a place in the game world that the player can travel to.
/// Each location holds a mutable list of <see cref="PointOfInterest"/>s that
/// the player can interact with.
/// </summary>
public class Location
{
    /// <summary>
    /// Unique identifier for this location.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Display name shown to the player (e.g. "Margery's House").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The points of interest currently present at this location.
    /// </summary>
    public List<PointOfInterest> PointsOfInterest { get; set; } = new();
}

