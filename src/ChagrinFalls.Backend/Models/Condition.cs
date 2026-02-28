namespace ChagrinFalls.Backend.Models;

/// <summary>
/// Represents a condition that must be met for a conversation event, dialogue line, or choice to be available.
/// </summary>
public class Condition
{
    /// <summary>
    /// The type of condition to evaluate (e.g. "ItemInInventory", "InformationLearned").
    /// </summary>
    public string ConditionType { get; set; } = string.Empty;

    /// <summary>
    /// Key-value parameters used by the condition evaluator.
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new();
}
