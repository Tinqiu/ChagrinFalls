using ChagrinFalls.Backend.Models;

namespace ChagrinFalls.Tests;

/// <summary>
/// Shared helpers for building test conversation fixtures.
/// </summary>
internal static class ConversationFixtures
{
    // ...existing code...

    /// <summary>
    /// Builds an event where line_1 has an AddItem effect for "key",
    /// then advances to line_2.
    ///   line_1 [AddItem: key] → line_2 → end
    /// </summary>
    public static ConversationEvent EventWithAddItemEffect(string itemId = "key")
    {
        var line1 = new DialogueLine
        {
            Id     = "line_1",
            Speaker = "NPC",
            Text   = "Take this.",
            NextDialogueLineId = "line_2",
            Effects = new List<ConversationEffect>
            {
                new() { EffectType = "AddItem", Parameters = new Dictionary<string, string> { ["itemId"] = itemId } }
            }
        };
        var line2 = new DialogueLine { Id = "line_2", Speaker = "NPC", Text = "Done." };
        return BuildSingleConvEvent("effect_event", line1, line2);
    }

    /// <summary>
    /// Builds an event where line_1 has a RemoveItem effect for "key",
    /// then advances to line_2.
    ///   line_1 [RemoveItem: key] → line_2 → end
    /// </summary>
    public static ConversationEvent EventWithRemoveItemEffect(string itemId = "key")
    {
        var line1 = new DialogueLine
        {
            Id     = "line_1",
            Speaker = "NPC",
            Text   = "Give it back.",
            NextDialogueLineId = "line_2",
            Effects = new List<ConversationEffect>
            {
                new() { EffectType = "RemoveItem", Parameters = new Dictionary<string, string> { ["itemId"] = itemId } }
            }
        };
        var line2 = new DialogueLine { Id = "line_2", Speaker = "NPC", Text = "Thanks." };
        return BuildSingleConvEvent("remove_effect_event", line1, line2);
    }

    /// <summary>
    /// Builds an event where line_1 has two effects: AddItem "sword" then RemoveItem "gold".
    ///   line_1 [AddItem: sword, RemoveItem: gold] → line_2 → end
    /// </summary>
    public static ConversationEvent EventWithMultipleEffects()
    {
        var line1 = new DialogueLine
        {
            Id     = "line_1",
            Speaker = "NPC",
            Text   = "A trade.",
            NextDialogueLineId = "line_2",
            Effects = new List<ConversationEffect>
            {
                new() { EffectType = "AddItem",    Parameters = new Dictionary<string, string> { ["itemId"] = "sword" } },
                new() { EffectType = "RemoveItem", Parameters = new Dictionary<string, string> { ["itemId"] = "gold"  } }
            }
        };
        var line2 = new DialogueLine { Id = "line_2", Speaker = "NPC", Text = "Done." };
        return BuildSingleConvEvent("multi_effect_event", line1, line2);
    }

    /// <summary>
    /// Builds an event where line_1 has a choice; the choice triggers an AddItem effect on line_1
    /// when selected (effect fires before navigation, not after).
    ///   line_1 [AddItem: key] → choice_a → line_2a
    /// </summary>
    public static ConversationEvent EventWithEffectOnChoiceLine(string itemId = "key")
    {
        var line2a = new DialogueLine { Id = "line_2a", Speaker = "NPC", Text = "You chose." };
        var line1 = new DialogueLine
        {
            Id     = "line_1",
            Speaker = "NPC",
            Text   = "Choose.",
            Effects = new List<ConversationEffect>
            {
                new() { EffectType = "AddItem", Parameters = new Dictionary<string, string> { ["itemId"] = itemId } }
            },
            Choices = new List<Choice>
            {
                new() { Id = "choice_a", Text = "Pick it up", NextDialogueLineId = "line_2a" }
            }
        };
        return BuildSingleConvEvent("choice_effect_event", line1, line2a);
    }

    // ── Private builder ───────────────────────────────────────────────────────

    private static ConversationEvent BuildSingleConvEvent(string eventId, params DialogueLine[] lines)
    {
        var dict = lines.ToDictionary(l => l.Id);
        var conv = new Conversation
        {
            Id = "conv_1",
            StartingDialogueLineId = lines[0].Id,
            DialogueLines = dict
        };
        return new ConversationEvent
        {
            Id = eventId,
            StartingConversationId = "conv_1",
            Conversations = new Dictionary<string, Conversation> { [conv.Id] = conv }
        };
    }

    /// <summary>
    /// Builds a minimal linear conversation event:
    ///   line_1 ("Hello!") → line_2 ("Goodbye.") → end
    /// No conditions on any element.
    /// </summary>
    public static ConversationEvent SimpleLinearEvent()
    {
        var line1 = new DialogueLine
        {
            Id = "line_1",
            Speaker = "NPC",
            Text = "Hello!",
            NextDialogueLineId = "line_2"
        };

        var line2 = new DialogueLine
        {
            Id = "line_2",
            Speaker = "NPC",
            Text = "Goodbye.",
            NextDialogueLineId = null
        };

        var conversation = new Conversation
        {
            Id = "conv_1",
            StartingDialogueLineId = "line_1",
            DialogueLines = new Dictionary<string, DialogueLine>
            {
                [line1.Id] = line1,
                [line2.Id] = line2
            }
        };

        return new ConversationEvent
        {
            Id = "event_1",
            StartingConversationId = "conv_1",
            Conversations = new Dictionary<string, Conversation>
            {
                [conversation.Id] = conversation
            }
        };
    }

    /// <summary>
    /// Builds a branching conversation event where the player chooses between two lines:
    ///   line_1 offers choice_a → line_2a ("Path A") → end
    ///                 choice_b → line_2b ("Path B") → end
    /// </summary>
    public static ConversationEvent BranchingEvent()
    {
        var line2a = new DialogueLine { Id = "line_2a", Speaker = "NPC", Text = "Path A" };
        var line2b = new DialogueLine { Id = "line_2b", Speaker = "NPC", Text = "Path B" };

        var line1 = new DialogueLine
        {
            Id = "line_1",
            Speaker = "NPC",
            Text = "Which path?",
            Choices = new List<Choice>
            {
                new() { Id = "choice_a", Text = "Go left",  NextDialogueLineId = "line_2a" },
                new() { Id = "choice_b", Text = "Go right", NextDialogueLineId = "line_2b" }
            }
        };

        var conversation = new Conversation
        {
            Id = "conv_1",
            StartingDialogueLineId = "line_1",
            DialogueLines = new Dictionary<string, DialogueLine>
            {
                [line1.Id]  = line1,
                [line2a.Id] = line2a,
                [line2b.Id] = line2b
            }
        };

        return new ConversationEvent
        {
            Id = "branching_event",
            StartingConversationId = "conv_1",
            Conversations = new Dictionary<string, Conversation>
            {
                [conversation.Id] = conversation
            }
        };
    }

    /// <summary>
    /// Builds an event where line_1 has one conditional branch (requires "key") pointing to
    /// line_branch, and a default NextDialogueLineId pointing to line_default.
    ///   line_1 ─[has key]→ line_branch → end
    ///          ─[default]→ line_default → end
    /// </summary>
    public static ConversationEvent EventWithSingleConditionalBranch(string requiredItem = "key")
    {
        var lineBranch  = new DialogueLine { Id = "line_branch",  Speaker = "NPC", Text = "Branch taken." };
        var lineDefault = new DialogueLine { Id = "line_default", Speaker = "NPC", Text = "Default taken." };
        var line1 = new DialogueLine
        {
            Id                 = "line_1",
            Speaker            = "NPC",
            Text               = "Which way?",
            NextDialogueLineId = "line_default",
            Branches = new List<ConditionalBranch>
            {
                new()
                {
                    Id                 = "branch_1",
                    NextDialogueLineId = "line_branch",
                    Conditions = new List<Condition>
                    {
                        new() { ConditionType = "ItemInInventory",
                                Parameters    = new Dictionary<string, string> { ["itemId"] = requiredItem } }
                    }
                }
            }
        };
        return BuildSingleConvEvent("conditional_branch_event", line1, lineBranch, lineDefault);
    }

    /// <summary>
    /// Builds an event where line_1 has two conditional branches — branch_a (requires "key_a")
    /// and branch_b (requires "key_b") — to verify first-match-wins ordering.
    ///   line_1 ─[has key_a]→ line_a → end
    ///          ─[has key_b]→ line_b → end
    ///          ─[default]  → line_default → end
    /// </summary>
    public static ConversationEvent EventWithTwoConditionalBranches()
    {
        var lineA       = new DialogueLine { Id = "line_a",       Speaker = "NPC", Text = "Branch A." };
        var lineB       = new DialogueLine { Id = "line_b",       Speaker = "NPC", Text = "Branch B." };
        var lineDefault = new DialogueLine { Id = "line_default", Speaker = "NPC", Text = "Default." };
        var line1 = new DialogueLine
        {
            Id                 = "line_1",
            Speaker            = "NPC",
            Text               = "Which way?",
            NextDialogueLineId = "line_default",
            Branches = new List<ConditionalBranch>
            {
                new()
                {
                    Id                 = "branch_a",
                    NextDialogueLineId = "line_a",
                    Conditions = new List<Condition>
                    {
                        new() { ConditionType = "ItemInInventory",
                                Parameters    = new Dictionary<string, string> { ["itemId"] = "key_a" } }
                    }
                },
                new()
                {
                    Id                 = "branch_b",
                    NextDialogueLineId = "line_b",
                    Conditions = new List<Condition>
                    {
                        new() { ConditionType = "ItemInInventory",
                                Parameters    = new Dictionary<string, string> { ["itemId"] = "key_b" } }
                    }
                }
            }
        };
        return BuildSingleConvEvent("two_branch_event", line1, lineA, lineB, lineDefault);
    }

    /// <summary>
    /// Builds an event where line_1 has a conditional branch (requires "key") that points to
    /// null — i.e. the branch ends the conversation immediately when taken.
    /// </summary>
    public static ConversationEvent EventWithBranchThatEndsConversation(string requiredItem = "key")
    {
        var lineDefault = new DialogueLine { Id = "line_default", Speaker = "NPC", Text = "Default." };
        var line1 = new DialogueLine
        {
            Id                 = "line_1",
            Speaker            = "NPC",
            Text               = "Bye?",
            NextDialogueLineId = "line_default",
            Branches = new List<ConditionalBranch>
            {
                new()
                {
                    Id                 = "branch_end",
                    NextDialogueLineId = null, // end the conversation
                    Conditions = new List<Condition>
                    {
                        new() { ConditionType = "ItemInInventory",
                                Parameters    = new Dictionary<string, string> { ["itemId"] = requiredItem } }
                    }
                }
            }
        };
        return BuildSingleConvEvent("branch_end_event", line1, lineDefault);
    }
}
