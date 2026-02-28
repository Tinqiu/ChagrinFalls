# Location Manager

`ChagrinFalls.Backend.Systems.LocationManager`

The `LocationManager` owns the world's locations, tracks where the player currently is, and handles the two state-changing operations: travelling between locations and picking up item points of interest.

---

## Constructor

```csharp
public LocationManager(GameState gameState, IEnumerable<Location> locations, string startingLocationId)
```

| Parameter | Description |
|---|---|
| `gameState` | The game state used when adding picked-up items to the inventory. |
| `locations` | All locations in the world. |
| `startingLocationId` | The ID of the location the player begins in. Case-insensitive. |

Throws `ArgumentNullException` for null `gameState` or `locations`, and `ArgumentException` if `startingLocationId` is null, whitespace, or not found in the supplied locations.

---

## Properties

### `CurrentLocation`
```csharp
public Location CurrentLocation { get; }
```
The location the player is currently at.

### `AllLocations`
```csharp
public IReadOnlyCollection<Location> AllLocations { get; }
```
All locations in the world, regardless of where the player currently is.

---

## Events

### `OnLocationChanged`
```csharp
public event Action<Location>? OnLocationChanged;
```
Raised after a successful `TravelTo` (with the new location) or after a successful `PickUpItem` (with the current location, now containing one fewer POI). Subscribe to this to keep the UI in sync.

---

## Methods

### `TravelTo`
```csharp
public void TravelTo(string locationId)
```
Moves the player to the specified location and raises `OnLocationChanged`. Throws `ArgumentException` if the location ID is unknown.

### `PickUpItem`
```csharp
public string PickUpItem(string poiId)
```
Picks up an item point of interest from the **current** location:

1. Removes the `PointOfInterest` from `CurrentLocation.PointsOfInterest`
2. Adds its `ItemId` to `GameState.Inventory`
3. Raises `OnLocationChanged`
4. Returns the item ID that was added

Throws `ArgumentException` when:
- the POI ID is not found at the current location
- the POI is a `Character` (not an `Item`)
- the POI has no `ItemId` set

---

## Models

### `Location`
```csharp
public class Location
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<PointOfInterest> PointsOfInterest { get; set; }
}
```

### `PointOfInterest`
```csharp
public class PointOfInterest
{
    public string Id { get; set; }
    public string Name { get; set; }
    public PointOfInterestType Type { get; set; }   // Character | Item
    public string? ConversationEventId { get; set; } // Character POIs
    public string? ItemId { get; set; }              // Item POIs
}
```

---

## Example

```csharp
var hut = new Location
{
    Id   = "hut_by_waterfall",
    Name = "Hut by the Waterfall",
    PointsOfInterest = new List<PointOfInterest>
    {
        new() { Id = "torn_letter_poi", Name = "Torn Letter",
                Type = PointOfInterestType.Item, ItemId = "torn_letter" }
    }
};

var manager = new LocationManager(gameState, [hut], startingLocationId: "hut_by_waterfall");

manager.OnLocationChanged += loc =>
    Console.WriteLine($"Now at: {loc.Name} ({loc.PointsOfInterest.Count} POIs)");

var itemId = manager.PickUpItem("torn_letter_poi");
// → event fires, inventory contains "torn_letter", POI list is empty
```

