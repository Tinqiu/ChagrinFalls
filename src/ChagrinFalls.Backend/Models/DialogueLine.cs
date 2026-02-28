namespace ChagrinFalls.Backend.Models;

/// <summary>
/// Represents a single line of dialogue that can be displayed to the player.
/// </summary>
public class DialogueLine
{
    /// <summary>
    /// Unique identifier for this dialogue line within a conversation.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the character who speaks this line (e.g. "Player", "Detective Moore").
    /// </summary>
    public string Speaker { get; set; } = string.Empty;

    /// <summary>
    /// The text content of this dialogue line.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Optional conditions that must all be met for this line to be shown.
    /// </summary>
    public List<Condition> Conditions { get; set; } = new();

    /// <summary>
    /// Player choices presented after this dialogue line.
    /// When no choices are available after condition filtering (including when <see cref="Choices"/> is empty),
    /// the conversation advances automatically to <see cref="NextDialogueLineId"/>.
    /// </summary>
    public List<Choice> Choices { get; set; } = new();

    /// <summary>
    /// The ID of the next dialogue line to display when no choices are available after condition filtering.
    /// Null indicates the conversation ends after this line.
    /// </summary>
    public string? NextDialogueLineId { get; set; }
}
