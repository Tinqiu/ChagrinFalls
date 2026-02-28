namespace ChagrinFalls.Backend.Models;

/// <summary>
/// Represents a conversation event — a top-level unit that groups one or more conversations,
/// defines who participates, and specifies conditions required to trigger the event.
/// </summary>
public class ConversationEvent
{
    /// <summary>
    /// Unique string identifier for this conversation event.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Conditions that must all be satisfied for this event to be available.
    /// </summary>
    public List<Condition> Conditions { get; set; } = new();

    /// <summary>
    /// The participants involved in this conversation event.
    /// </summary>
    public List<Participant> Participants { get; set; } = new();

    /// <summary>
    /// The conversations that make up this event, keyed by conversation ID.
    /// </summary>
    public Dictionary<string, Conversation> Conversations { get; set; } = new();

    /// <summary>
    /// The ID of the first conversation to start when this event is triggered.
    /// </summary>
    public string StartingConversationId { get; set; } = string.Empty;
}
