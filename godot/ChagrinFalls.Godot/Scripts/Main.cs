using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;
using Godot;

namespace ChagrinFalls.Godot.Scripts;

/// <summary>
/// Root scene script. Bootstraps game state and the location system,
/// then wires up the location screen, dialogue UI, and inventory screen.
/// </summary>
public partial class Main : Control
{
    private DialogueUI       _dialogueUI      = null!;
    private InventoryScreen  _inventoryScreen = null!;
    private LocationScreen   _locationScreen  = null!;
    private TravelScreen     _travelScreen    = null!;

    private GameState        _gameState        = null!;
    private LocationManager  _locationManager  = null!;

    // Conversation events keyed by ID so the location screen can look them up.
    private Dictionary<string, ConversationEvent> _conversationEvents = new();

    public override void _Ready()
    {
        _dialogueUI      = GetNode<DialogueUI>("DialogueUI");
        _inventoryScreen = GetNode<InventoryScreen>("InventoryScreen");
        _locationScreen  = GetNode<LocationScreen>("LocationScreen");
        _travelScreen    = GetNode<TravelScreen>("TravelScreen");

        var menuBar = GetNode<MenuBar>("MenuBar");
        menuBar.BackpackPressed += OnBackpackPressed;
        menuBar.TravelPressed   += OnTravelPressed;

        _gameState = new GameState();

        _conversationEvents = BuildConversationEvents();
        _locationManager    = BuildLocationManager();

        _locationScreen.Initialise(_locationManager);
        _locationScreen.CharacterInteracted += OnCharacterInteracted;

        _dialogueUI.ConversationFinished += OnConversationFinished;
    }

    // ── World building ────────────────────────────────────────────────────────

    private Dictionary<string, ConversationEvent> BuildConversationEvents()
    {
        var evt = BuildMargeryConversationEvent();
        return new Dictionary<string, ConversationEvent>
        {
            [evt.Id] = evt
        };
    }

    private LocationManager BuildLocationManager()
    {
        var margerysHouse = new Location
        {
            Id   = "margerys_house",
            Name = "Margery's House",
            PointsOfInterest = new List<PointOfInterest>
            {
                new()
                {
                    Id                 = "margery",
                    Name               = "Margery",
                    Type               = PointOfInterestType.Character,
                    ConversationEventId = "event_margery",
                }
            }
        };

        var hutByWaterfall = new Location
        {
            Id   = "hut_by_waterfall",
            Name = "Hut by the Waterfall",
            PointsOfInterest = new List<PointOfInterest>
            {
                new()
                {
                    Id     = "torn_letter_poi",
                    Name   = "Torn Letter",
                    Type   = PointOfInterestType.Item,
                    ItemId = "torn_letter",
                }
            }
        };

        return new LocationManager(
            _gameState,
            [margerysHouse, hutByWaterfall],
            startingLocationId: "margerys_house");
    }

    /// <summary>
    /// Margery conversation:
    ///   Detective greets Margery
    ///     → "Do you know about the spooky shack?" → Margery denies it
    ///           → "Ah that's unfortunate."              → loops back to greeting
    ///           → [torn letter] "This letter begs to differ!" → Margery confesses → end
    ///     → "I have nothing else to ask, goodbye."  → end
    /// </summary>
    private static ConversationEvent BuildMargeryConversationEvent()
    {
        // ── End ──────────────────────────────────────────────────────────────
        var lineEndArrest = new DialogueLine
        {
            Id      = "line_end_arrest",
            Speaker = "Detective Moore",
            Text    = "You're under arrest, old bag. You're coming with me.",
        };

        var lineEndGoodbye = new DialogueLine
        {
            Id      = "line_end_goodbye",
            Speaker = "Detective Moore",
            Text    = "I'll be in touch, Margery.",
        };

        // ── Confrontation branch ─────────────────────────────────────────────
        var lineMargeryConfesses = new DialogueLine
        {
            Id                 = "line_margery_confesses",
            Speaker            = "Margery",
            Text               = "Blast it, you got me!",
            NextDialogueLineId = "line_end_arrest",
        };

        var lineLetterAccuse = new DialogueLine
        {
            Id      = "line_letter_accuse",
            Speaker = "Detective Moore",
            Text    = "This letter with your signature on it begs to differ!",
            NextDialogueLineId = "line_margery_confesses",
        };

        // ── Denial branch ────────────────────────────────────────────────────
        var lineMargeryDenies = new DialogueLine
        {
            Id      = "line_margery_denies",
            Speaker = "Margery",
            Text    = "I have no idea what you're talking about. I don't know anything about a spooky shack in the woods.",
            Choices = new List<Choice>
            {
                new()
                {
                    Id                 = "choice_unfortunate",
                    Text               = "Ah, that's unfortunate.",
                    NextDialogueLineId = "line_greeting",   // loops back
                },
                new()
                {
                    Id                 = "choice_torn_letter",
                    Text               = "[Torn Letter] This letter with your signature on it begs to differ!",
                    NextDialogueLineId = "line_letter_accuse",
                    Conditions = new List<Condition>
                    {
                        new()
                        {
                            ConditionType = "ItemInInventory",
                            Parameters    = new Dictionary<string, string> { ["itemId"] = "torn_letter" }
                        }
                    }
                },
            }
        };

        // ── Greeting ─────────────────────────────────────────────────────────
        var lineGreeting = new DialogueLine
        {
            Id                 = "line_greeting",
            Speaker            = "Detective Moore",
            Text               = "Good evening, Margery. I'm Detective Moore — I'm investigating some strange goings-on around Chagrin Falls.",
            NextDialogueLineId = "line_greeting_2",
        };

        var lineGreeting2 = new DialogueLine
        {
            Id      = "line_greeting_2",
            Speaker = "Margery",
            Text    = "Detective. What can I do for you?",
            Choices = new List<Choice>
            {
                new()
                {
                    Id                 = "choice_shack",
                    Text               = "Do you know anything about the spooky shack in the woods?",
                    NextDialogueLineId = "line_margery_denies",
                },
                new()
                {
                    Id                 = "choice_goodbye",
                    Text               = "I have nothing else to ask. Goodbye.",
                    NextDialogueLineId = "line_end_goodbye",
                },
            }
        };

        var conversation = new Conversation
        {
            Id                     = "conv_margery",
            StartingDialogueLineId = "line_greeting",
            DialogueLines = new Dictionary<string, DialogueLine>
            {
                [lineGreeting.Id]          = lineGreeting,
                [lineGreeting2.Id]         = lineGreeting2,
                [lineMargeryDenies.Id]     = lineMargeryDenies,
                [lineLetterAccuse.Id]      = lineLetterAccuse,
                [lineMargeryConfesses.Id]  = lineMargeryConfesses,
                [lineEndArrest.Id]         = lineEndArrest,
                [lineEndGoodbye.Id]        = lineEndGoodbye,
            }
        };

        return new ConversationEvent
        {
            Id                     = "event_margery",
            StartingConversationId = "conv_margery",
            Conversations = new Dictionary<string, Conversation>
            {
                [conversation.Id] = conversation
            }
        };
    }

    // ── Event handlers ────────────────────────────────────────────────────────

    private void OnCharacterInteracted(string poiId)
    {
        var poi = _locationManager.CurrentLocation.PointsOfInterest
            .FirstOrDefault(p => string.Equals(p.Id, poiId, StringComparison.OrdinalIgnoreCase));

        if (poi?.ConversationEventId is null ||
            !_conversationEvents.TryGetValue(poi.ConversationEventId, out var evt))
        {
            GD.PrintErr($"No conversation event found for POI '{poiId}'.");
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
        _inventoryScreen.Open(_gameState.Inventory);
    }

    private void OnTravelPressed()
    {
        _travelScreen.Open(_locationManager);
    }
}
