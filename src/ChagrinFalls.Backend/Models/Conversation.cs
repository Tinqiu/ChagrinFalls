namespace ChagrinFalls.Backend.Models;

/// <summary>
/// Represents a conversation composed of one or more dialogue lines.
/// Dialogue lines can branch based on player choices and conditions.
/// </summary>
public class Conversation
{
    /// <summary>
    /// Unique identifier for this conversation.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// All dialogue lines that belong to this conversation, keyed by their ID.
    /// </summary>
    public Dictionary<string, DialogueLine> DialogueLines { get; set; } = new();

    /// <summary>
    /// The ID of the first dialogue line to display when this conversation starts.
    /// </summary>
    public string StartingDialogueLineId { get; set; } = string.Empty;
}
