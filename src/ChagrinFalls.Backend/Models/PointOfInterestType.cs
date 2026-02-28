namespace ChagrinFalls.Backend.Models;

/// <summary>
/// Distinguishes what kind of point of interest this is,
/// which determines how the player can interact with it.
/// </summary>
public enum PointOfInterestType
{
    /// <summary>An NPC the player can talk to.</summary>
    Character,

    /// <summary>An item the player can pick up.</summary>
    Item,
}

