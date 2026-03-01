using ChagrinFalls.Backend.Models;

namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Tracks important pieces of information (clues, facts, story beats) that the player has learned.
/// Used to evaluate <c>InformationLearned</c> conditions during conversations.
/// </summary>
public class PlayerJournal
{
    private readonly Dictionary<string, JournalEntry> _journalEntries = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Records a new journal entry in the journal.
    /// </summary>
    /// <param name="entry">The journal entry to record.</param>
    public void LearnInformation(JournalEntry entry)
    {
        if (entry == null)
            throw new ArgumentNullException(nameof(entry));
        if (string.IsNullOrWhiteSpace(entry.Id))
            throw new ArgumentException("Journal entry ID cannot be null or whitespace.", nameof(entry.Id));

        _journalEntries[entry.Id] = entry;
    }

    /// <summary>
    /// Returns whether the player has learned the specified piece of information.
    /// </summary>
    /// <param name="informationId">The unique identifier of the information to check.</param>
    public bool HasLearned(string? informationId) =>
        !string.IsNullOrWhiteSpace(informationId) && _journalEntries.ContainsKey(informationId);

    /// <summary>
    /// Returns all journal entries currently recorded in the journal.
    /// </summary>
    public IReadOnlyCollection<JournalEntry> GetAllLearnedInformation() =>
        _journalEntries.Values.ToList().AsReadOnly();

    /// <summary>
    /// Returns a specific journal entry by its ID, or null if not found.
    /// </summary>
    /// <param name="informationId">The unique identifier of the journal entry to retrieve.</param>
    public JournalEntry? GetJournalEntry(string? informationId) =>
        !string.IsNullOrWhiteSpace(informationId) && _journalEntries.TryGetValue(informationId, out var entry)
            ? entry
            : null;
}
