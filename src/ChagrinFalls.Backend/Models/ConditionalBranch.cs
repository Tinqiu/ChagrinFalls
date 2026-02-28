namespace ChagrinFalls.Backend.Models;
/// <summary>
/// A conditional branch on a <see cref="DialogueLine"/>.
/// When all <see cref="Conditions"/> are met, the conversation advances to
/// <see cref="NextDialogueLineId"/> instead of the line's default next line.
/// Branches are evaluated in order; the first matching branch wins.
/// </summary>
public class ConditionalBranch
{
    /// <summary>Unique identifier for this branch within its dialogue line.</summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>
    /// All conditions must be met for this branch to be taken.
    /// An empty list means the branch always matches (use as an unconditional fallback).
    /// </summary>
    public List<Condition> Conditions { get; set; } = new();
    /// <summary>The dialogue line to navigate to when this branch is taken.
    /// Null ends the conversation.</summary>
    public string? NextDialogueLineId { get; set; }
}
