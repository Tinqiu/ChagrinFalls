using ChagrinFalls.Backend.Models;

namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Manages the world's locations and tracks where the player currently is.
/// Handles travel between locations and item pickup from points of interest.
/// </summary>
public class LocationManager
{
    private readonly Dictionary<string, Location> _locations;
    private readonly GameState _gameState;

    /// <summary>
    /// Raised when the player travels to a new location, or when the current
    /// location's points of interest change (e.g. after picking up an item).
    /// The argument is the new current location.
    /// </summary>
    public event Action<Location>? OnLocationChanged;

    /// <param name="gameState">The game state used when picking up items.</param>
    /// <param name="locations">All locations in the world.</param>
    /// <param name="startingLocationId">The ID of the location the player begins in.</param>
    public LocationManager(GameState gameState, IEnumerable<Location> locations, string startingLocationId)
    {
        ArgumentNullException.ThrowIfNull(gameState);
        ArgumentNullException.ThrowIfNull(locations);

        _gameState = gameState;
        _locations = locations.ToDictionary(l => l.Id, StringComparer.OrdinalIgnoreCase);

        ArgumentException.ThrowIfNullOrWhiteSpace(startingLocationId);

        if (!_locations.TryGetValue(startingLocationId, out var start))
            throw new ArgumentException(
                $"Starting location '{startingLocationId}' was not found.", nameof(startingLocationId));

        CurrentLocation = start;
    }

    /// <summary>The location the player is currently at.</summary>
    public Location CurrentLocation { get; private set; }

    /// <summary>All locations in the world.</summary>
    public IReadOnlyCollection<Location> AllLocations => _locations.Values;

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Moves the player to the specified location.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the location ID is unknown.</exception>
    public void TravelTo(string locationId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);
        if (!_locations.TryGetValue(locationId, out var location))
            throw new ArgumentException($"Location '{locationId}' was not found.", nameof(locationId));

        CurrentLocation = location;
        OnLocationChanged?.Invoke(CurrentLocation);
    }

    /// <summary>
    /// Picks up an item point of interest from the current location:
    /// removes it from the location's <see cref="Location.PointsOfInterest"/> and
    /// adds its <see cref="PointOfInterest.ItemId"/> to the player's inventory.
    /// </summary>
    /// <param name="poiId">The ID of the item POI to pick up.</param>
    /// <returns>The item ID that was added to the inventory.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the POI is not found at the current location, is not an item,
    /// or has no <see cref="PointOfInterest.ItemId"/> set.
    /// </exception>
    public string PickUpItem(string poiId)
    {
        var poi = CurrentLocation.PointsOfInterest
            .FirstOrDefault(p => string.Equals(p.Id, poiId, StringComparison.OrdinalIgnoreCase));

        if (poi is null)
            throw new ArgumentException(
                $"POI '{poiId}' was not found at location '{CurrentLocation.Id}'.", nameof(poiId));

        if (poi.Type != PointOfInterestType.Item)
            throw new ArgumentException(
                $"POI '{poiId}' is not an item and cannot be picked up.", nameof(poiId));

        if (string.IsNullOrWhiteSpace(poi.ItemId))
            throw new ArgumentException(
                $"Item POI '{poiId}' has no ItemId set.", nameof(poiId));

        CurrentLocation.PointsOfInterest.Remove(poi);
        _gameState.Inventory.AddItem(poi.ItemId);

        OnLocationChanged?.Invoke(CurrentLocation);

        return poi.ItemId;
    }
}

