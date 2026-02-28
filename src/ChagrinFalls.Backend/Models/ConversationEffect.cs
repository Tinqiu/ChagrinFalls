namespace ChagrinFalls.Backend.Models;
/// <summary>
/// An effect that is applied when the player advances past a <see cref="DialogueLine"/>.
/// </summary>
public class ConversationEffect
{
    /// <summary>
    /// The type of effect to apply (e.g. "AddItem", "RemoveItem").
    /// </summary>
    public string EffectType { get; set; } = string.Empty;
    /// <summary>
    /// Key-value parameters used by the effect handler (e.g. "itemId" → "torn_letter").
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new();
}
