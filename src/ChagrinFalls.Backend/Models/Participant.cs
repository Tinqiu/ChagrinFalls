namespace ChagrinFalls.Backend.Models;

/// <summary>
/// Represents a participant in a conversation event (e.g. the Player or an NPC).
/// </summary>
public class Participant
{
    /// <summary>
    /// The type of participant (e.g. "Player", "NPC").
    /// </summary>
    public string ParticipantType { get; set; } = string.Empty;

    /// <summary>
    /// Optional key-value parameters that further describe the participant (e.g. NPC id, name).
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new();
}
