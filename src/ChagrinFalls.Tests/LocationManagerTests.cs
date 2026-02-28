using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class LocationManagerTests
{
    // ── Fixtures ──────────────────────────────────────────────────────────────

    private static Location MakeLocation(string id, string name, params PointOfInterest[] pois) =>
        new() { Id = id, Name = name, PointsOfInterest = pois.ToList() };

    private static PointOfInterest MakeItemPoi(string id, string itemId) =>
        new() { Id = id, Name = id, Type = PointOfInterestType.Item, ItemId = itemId };

    private static PointOfInterest MakeCharacterPoi(string id) =>
        new() { Id = id, Name = id, Type = PointOfInterestType.Character, ConversationEventId = "evt_1" };

    private static LocationManager CreateManager(
        GameState? state = null,
        string startId = "loc_a",
        params Location[] extraLocations)
    {
        var locA = MakeLocation("loc_a", "Location A");
        var locations = new[] { locA }.Concat(extraLocations);
        return new LocationManager(state ?? new GameState(), locations, startId);
    }

    // ── Constructor ───────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_SetsCurrentLocation_ToStartingLocation()
    {
        var manager = CreateManager();
        Assert.Equal("loc_a", manager.CurrentLocation.Id);
    }

    [Fact]
    public void Constructor_Throws_WhenGameStateIsNull()
    {
        var loc = MakeLocation("loc_a", "A");
        Assert.Throws<ArgumentNullException>(() =>
            new LocationManager(null!, [loc], "loc_a"));
    }

    [Fact]
    public void Constructor_Throws_WhenLocationsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LocationManager(new GameState(), null!, "loc_a"));
    }

    [Fact]
    public void Constructor_Throws_WhenStartingLocationIdIsNull()
    {
        var loc = MakeLocation("loc_a", "A");
        Assert.Throws<ArgumentNullException>(() =>
            new LocationManager(new GameState(), [loc], null!));
    }

    [Fact]
    public void Constructor_Throws_WhenStartingLocationIdIsWhitespace()
    {
        var loc = MakeLocation("loc_a", "A");
        Assert.Throws<ArgumentException>(() =>
            new LocationManager(new GameState(), [loc], "   "));
    }

    [Fact]
    public void Constructor_Throws_WhenStartingLocationIdNotFound()
    {
        var loc = MakeLocation("loc_a", "A");
        Assert.Throws<ArgumentException>(() =>
            new LocationManager(new GameState(), [loc], "does_not_exist"));
    }

    [Fact]
    public void AllLocations_ContainsEverySuppliedLocation()
    {
        var locA = MakeLocation("loc_a", "A");
        var locB = MakeLocation("loc_b", "B");
        var manager = new LocationManager(new GameState(), [locA, locB], "loc_a");

        Assert.Equal(2, manager.AllLocations.Count);
        Assert.Contains(manager.AllLocations, l => l.Id == "loc_a");
        Assert.Contains(manager.AllLocations, l => l.Id == "loc_b");
    }

    // ── TravelTo ──────────────────────────────────────────────────────────────

    [Fact]
    public void TravelTo_UpdatesCurrentLocation()
    {
        var locB = MakeLocation("loc_b", "Location B");
        var manager = CreateManager(extraLocations: locB);

        manager.TravelTo("loc_b");

        Assert.Equal("loc_b", manager.CurrentLocation.Id);
    }

    [Fact]
    public void TravelTo_IsCaseInsensitive()
    {
        var locB = MakeLocation("loc_b", "Location B");
        var manager = CreateManager(extraLocations: locB);

        manager.TravelTo("LOC_B");

        Assert.Equal("loc_b", manager.CurrentLocation.Id);
    }

    [Fact]
    public void TravelTo_RaisesOnLocationChanged_WithNewLocation()
    {
        var locB = MakeLocation("loc_b", "Location B");
        var manager = CreateManager(extraLocations: locB);

        Location? received = null;
        manager.OnLocationChanged += l => received = l;

        manager.TravelTo("loc_b");

        Assert.NotNull(received);
        Assert.Equal("loc_b", received!.Id);
    }

    [Fact]
    public void TravelTo_Throws_WhenLocationIdNotFound()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentException>(() => manager.TravelTo("nowhere"));
    }

    [Fact]
    public void TravelTo_DoesNotRaiseOnLocationChanged_WhenThrows()
    {
        var manager = CreateManager();
        var raised = false;
        manager.OnLocationChanged += _ => raised = true;

        Assert.Throws<ArgumentException>(() => manager.TravelTo("nowhere"));
        Assert.False(raised);
    }

    // ── PickUpItem ────────────────────────────────────────────────────────────

    [Fact]
    public void PickUpItem_ReturnsItemId()
    {
        var loc = MakeLocation("loc_a", "A", MakeItemPoi("poi_key", "brass_key"));
        var manager = new LocationManager(new GameState(), [loc], "loc_a");

        var result = manager.PickUpItem("poi_key");

        Assert.Equal("brass_key", result);
    }

    [Fact]
    public void PickUpItem_AddsItemToInventory()
    {
        var state = new GameState();
        var loc = MakeLocation("loc_a", "A", MakeItemPoi("poi_key", "brass_key"));
        var manager = new LocationManager(state, [loc], "loc_a");

        manager.PickUpItem("poi_key");

        Assert.True(state.Inventory.HasItem("brass_key"));
    }

    [Fact]
    public void PickUpItem_RemovesPoisFromCurrentLocation()
    {
        var loc = MakeLocation("loc_a", "A", MakeItemPoi("poi_key", "brass_key"));
        var manager = new LocationManager(new GameState(), [loc], "loc_a");

        manager.PickUpItem("poi_key");

        Assert.Empty(manager.CurrentLocation.PointsOfInterest);
    }

    [Fact]
    public void PickUpItem_RaisesOnLocationChanged_WithCurrentLocation()
    {
        var loc = MakeLocation("loc_a", "A", MakeItemPoi("poi_key", "brass_key"));
        var manager = new LocationManager(new GameState(), [loc], "loc_a");

        Location? received = null;
        manager.OnLocationChanged += l => received = l;

        manager.PickUpItem("poi_key");

        Assert.NotNull(received);
        Assert.Equal("loc_a", received!.Id);
    }

    [Fact]
    public void PickUpItem_IsCaseInsensitive()
    {
        var loc = MakeLocation("loc_a", "A", MakeItemPoi("poi_key", "brass_key"));
        var manager = new LocationManager(new GameState(), [loc], "loc_a");

        manager.PickUpItem("POI_KEY");

        Assert.True(manager.CurrentLocation.PointsOfInterest.Count == 0);
    }

    [Fact]
    public void PickUpItem_Throws_WhenPoiNotFound()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentException>(() => manager.PickUpItem("nonexistent"));
    }

    [Fact]
    public void PickUpItem_Throws_WhenPoiIsCharacter()
    {
        var loc = MakeLocation("loc_a", "A", MakeCharacterPoi("npc_margery"));
        var manager = new LocationManager(new GameState(), [loc], "loc_a");

        Assert.Throws<ArgumentException>(() => manager.PickUpItem("npc_margery"));
    }

    [Fact]
    public void PickUpItem_Throws_WhenPoiHasNoItemId()
    {
        var poi = new PointOfInterest { Id = "poi_empty", Type = PointOfInterestType.Item, ItemId = null };
        var loc = MakeLocation("loc_a", "A", poi);
        var manager = new LocationManager(new GameState(), [loc], "loc_a");

        Assert.Throws<ArgumentException>(() => manager.PickUpItem("poi_empty"));
    }

    [Fact]
    public void PickUpItem_Throws_WhenPoiIsNotInCurrentLocation()
    {
        var locA = MakeLocation("loc_a", "A");
        var locB = MakeLocation("loc_b", "B", MakeItemPoi("poi_key", "brass_key"));
        var manager = new LocationManager(new GameState(), [locA, locB], "loc_a");

        // poi_key belongs to loc_b, but we're at loc_a
        Assert.Throws<ArgumentException>(() => manager.PickUpItem("poi_key"));
    }

    [Fact]
    public void PickUpItem_DoesNotModifyInventoryOrLocation_WhenThrows()
    {
        var state = new GameState();
        var manager = CreateManager(state);

        Assert.Throws<ArgumentException>(() => manager.PickUpItem("nonexistent"));

        Assert.Empty(state.Inventory.GetAllItems());
    }
}

