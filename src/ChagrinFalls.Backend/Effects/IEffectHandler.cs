namespace ChagrinFalls.Backend.Effects;
using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;
/// <summary>
/// Applies a <see cref="ConversationEffect"/> to the current <see cref="GameState"/>.
/// </summary>
public interface IEffectHandler
{
    /// <summary>The effect type this handler is responsible for (e.g. "AddItem").</summary>
    string EffectType { get; }
    /// <summary>Apply the effect.</summary>
    void Apply(ConversationEffect effect, GameState gameState);
}
