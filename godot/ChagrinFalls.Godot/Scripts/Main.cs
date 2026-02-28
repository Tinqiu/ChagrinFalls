using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;
using Godot;

namespace ChagrinFalls.Godot.Scripts;

/// <summary>
/// Root scene script. Bootstraps the game state, builds a sample conversation event,
/// and wires it up to the <see cref="DialogueUI"/> to demonstrate the dialogue system.
/// </summary>
public partial class Main : Node
{
    [Export] private DialogueUI _dialogueUI = null!;

    private GameState _gameState = null!;

    public override void _Ready()
    {
        _gameState = new GameState();

        // Give the player a sample item so the conditional choice is visible.
        _gameState.Inventory.AddItem("torn_letter");

        _dialogueUI.ConversationFinished += OnConversationFinished;

        StartSampleConversation();
    }

    // ── Sample conversation ───────────────────────────────────────────────────

    private void StartSampleConversation()
    {
        var manager = new DialogueManager(_gameState);
        var evt     = BuildSampleEvent();
        _dialogueUI.StartConversation(manager, evt);
    }

    /// <summary>
    /// Constructs a small demonstration conversation that exercises branching and
    /// a condition-gated dialogue choice.
    /// </summary>
    private static ConversationEvent BuildSampleEvent()
    {
        var lineEnd = new DialogueLine
        {
            Id       = "line_end",
            Speaker  = "Detective Moore",
            Text     = "I'll be in touch.",
        };

        var lineB = new DialogueLine
        {
            Id       = "line_b",
            Speaker  = "Detective Moore",
            Text     = "That letter could be important. Keep it safe.",
            NextDialogueLineId = "line_end"
        };

        var lineA = new DialogueLine
        {
            Id       = "line_a",
            Speaker  = "Detective Moore",
            Text     = "Very well. I have nothing more to say right now.",
            NextDialogueLineId = "line_end"
        };

        var lineGreeting = new DialogueLine
        {
            Id      = "line_greeting",
            Speaker = "Detective Moore",
            Text    = "Good evening. I'm investigating the disappearance at Chagrin Falls. Do you know anything?",
            Choices = new List<Choice>
            {
                new()
                {
                    Id                 = "choice_nothing",
                    Text               = "I don't know anything.",
                    NextDialogueLineId = "line_a"
                },
                new()
                {
                    Id                 = "choice_letter",
                    Text               = "I found a torn letter near the falls.",
                    NextDialogueLineId = "line_b",
                    // Only shown if the player actually has the letter.
                    Conditions = new List<Condition>
                    {
                        new()
                        {
                            ConditionType = "ItemInInventory",
                            Parameters    = new Dictionary<string, string> { ["itemId"] = "torn_letter" }
                        }
                    }
                }
            }
        };

        var conversation = new Conversation
        {
            Id                     = "conv_intro",
            StartingDialogueLineId = "line_greeting",
            DialogueLines = new Dictionary<string, DialogueLine>
            {
                [lineGreeting.Id] = lineGreeting,
                [lineA.Id]        = lineA,
                [lineB.Id]        = lineB,
                [lineEnd.Id]      = lineEnd,
            }
        };

        return new ConversationEvent
        {
            Id                     = "event_intro",
            StartingConversationId = "conv_intro",
            Conversations = new Dictionary<string, Conversation>
            {
                [conversation.Id] = conversation
            }
        };
    }

    private void OnConversationFinished()
    {
        GD.Print("Conversation finished.");
    }
}
