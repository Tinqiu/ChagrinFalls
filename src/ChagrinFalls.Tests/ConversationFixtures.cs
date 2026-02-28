using ChagrinFalls.Backend.Models;


namespace ChagrinFalls.Tests;

/// <summary>
/// Shared helpers for building test conversation fixtures.
/// </summary>
internal static class ConversationFixtures
{
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
}
