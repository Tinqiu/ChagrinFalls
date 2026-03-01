namespace ChagrinFalls.Backend.Models;

/// <summary>
/// A journal entry that captures important information the player has learned.
/// Used to track clues, facts, and story beats in the player's journal.
/// </summary>
public class JournalEntry
{
    /// <summary>Unique identifier for the journal entry (e.g. "found_clue_1").</summary>
    public string Id { get; set; } = string.Empty;
    /// <summary>Display title of the journal entry shown to the player (e.g. "Strange Letter Found").</summary>
    public string Title { get; set; } = string.Empty;
    /// <summary>Description or content of the journal entry.</summary>
    public string Description { get; set; } = string.Empty;
}

