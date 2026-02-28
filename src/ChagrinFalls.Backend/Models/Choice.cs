namespace ChagrinFalls.Backend.Models;

/// <summary>
/// Represents a player choice that can be presented during a dialogue line.
/// </summary>
public class Choice
{
    /// <summary>
    /// Unique identifier for this choice.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The text of the choice as displayed to the player.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Optional conditions that must all be met for this choice to be available to the player.
    /// </summary>
    public List<Condition> Conditions { get; set; } = new();

    /// <summary>
    /// The ID of the dialogue line to transition to when this choice is selected.
    /// Null indicates the conversation ends after this choice.
    /// </summary>
    public string? NextDialogueLineId { get; set; }
}
