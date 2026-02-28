namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Tracks important pieces of information (clues, facts, story beats) that the player has learned.
/// Used to evaluate <c>InformationLearned</c> conditions during conversations.
/// </summary>
public class PlayerJournal
{
    private readonly HashSet<string> _learnedInformation = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Records a new piece of information in the journal.
    /// </summary>
    /// <param name="informationId">The unique identifier of the information to record.</param>
    public void LearnInformation(string informationId)
    {
        if (string.IsNullOrWhiteSpace(informationId))
            throw new ArgumentException("Information ID cannot be null or whitespace.", nameof(informationId));

        _learnedInformation.Add(informationId);
    }

    /// <summary>
    /// Returns whether the player has learned the specified piece of information.
    /// </summary>
    /// <param name="informationId">The unique identifier of the information to check.</param>
    public bool HasLearned(string informationId) =>
        !string.IsNullOrWhiteSpace(informationId) && _learnedInformation.Contains(informationId);

    /// <summary>
    /// Returns all information IDs currently recorded in the journal.
    /// </summary>
    public IReadOnlyCollection<string> GetAllLearnedInformation() =>
        _learnedInformation.ToList().AsReadOnly();
}
