using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;
using Godot;

namespace ChagrinFalls.Godot.Scripts;

/// <summary>
/// Root scene script. Loads storybooks from disk, presents the selection screen,
/// then bootstraps game state and the location system when one is chosen.
/// </summary>
public partial class Main : Control
{
    private DialogueUI              _dialogueUI             = null!;
    private InventoryScreen         _inventoryScreen        = null!;
    private LocationScreen          _locationScreen         = null!;
    private TravelScreen            _travelScreen           = null!;
    private StorybookSelectScreen   _storybookSelectScreen  = null!;

    private GameState?       _gameState;
    private LocationManager? _locationManager;
    private Storybook?       _activeStorybook;

    private IReadOnlyDictionary<string, Storybook> _storybooks = new Dictionary<string, Storybook>();

    public override void _Ready()
    {
        _dialogueUI            = GetNode<DialogueUI>("DialogueUI");
        _inventoryScreen       = GetNode<InventoryScreen>("InventoryScreen");
        _locationScreen        = GetNode<LocationScreen>("LocationScreen");
        _travelScreen          = GetNode<TravelScreen>("TravelScreen");
        _storybookSelectScreen = GetNode<StorybookSelectScreen>("StorybookSelectScreen");

        var menuBar = GetNode<MenuBar>("MenuBar");
        menuBar.BackpackPressed += OnBackpackPressed;
        menuBar.TravelPressed   += OnTravelPressed;

        _dialogueUI.ConversationFinished += OnConversationFinished;
        _locationScreen.CharacterInteracted += OnCharacterInteracted;
        _storybookSelectScreen.StorybookSelected += OnStorybookSelected;

        LoadStorybooks();
        _storybookSelectScreen.Populate(_storybooks.Values.ToList());
    }

    // ── Storybook loading ─────────────────────────────────────────────────────

    private void LoadStorybooks()
    {
        var storybooksDir = System.IO.Path.Combine(
            ProjectSettings.GlobalizePath("res://"), "Storybooks");

        var loader = new StorybookLoader();
        var loaded = loader.LoadAll(storybooksDir, (file, ex) =>
            GD.PrintErr($"Failed to load storybook '{file}': {ex.Message}"));

        _storybooks = loaded.ToDictionary(s => s.Id);
    }

    // ── Game initialisation ───────────────────────────────────────────────────

    private void OnStorybookSelected(string storybookId)
    {
        if (!_storybooks.TryGetValue(storybookId, out var storybook))
        {
            GD.PrintErr($"Selected storybook '{storybookId}' not found.");
            return;
        }

        _gameState       = new GameState();
        _activeStorybook = storybook;

        _locationManager = new LocationManager(
            _gameState,
            storybook.Locations.Values,
            storybook.StartingLocationId);

        _locationScreen.Initialise(_locationManager);
    }

    // ── Event handlers ────────────────────────────────────────────────────────

    private void OnCharacterInteracted(string poiId)
    {
        if (_locationManager is null || _gameState is null || _activeStorybook is null) return;

        var poi = _locationManager.CurrentLocation.PointsOfInterest
            .FirstOrDefault(p => string.Equals(p.Id, poiId, StringComparison.OrdinalIgnoreCase));

        if (poi is null)
        {
            GD.PrintErr($"POI '{poiId}' not found in current location.");
            return;
        }

        if (string.IsNullOrEmpty(poi.ConversationEventId))
        {
            GD.PrintErr($"POI '{poiId}' ({poi.Name}) has no conversation event assigned.");
            _dialogueUI.ShowMessage(
                poi.Name,
                $"{poi.Name} doesn't seem to want to talk right now.");
            return;
        }

        if (!_activeStorybook.ConversationEvents.TryGetValue(poi.ConversationEventId, out var evt))
        {
            GD.PrintErr($"Conversation event '{poi.ConversationEventId}' not found in storybook.");
            _dialogueUI.ShowMessage(
                poi.Name,
                $"(Missing conversation event '{poi.ConversationEventId}')");
            return;
        }

        var manager = new DialogueManager(_gameState);
        _dialogueUI.StartConversation(manager, evt);
    }

    private void OnConversationFinished()
    {
        GD.Print("Conversation finished.");
    }

    private void OnBackpackPressed()
    {
        if (_gameState is null) return;
        _inventoryScreen.Open(_gameState.Inventory);
    }

    private void OnTravelPressed()
    {
        if (_locationManager is null) return;
        _travelScreen.Open(_locationManager);
    }
}
