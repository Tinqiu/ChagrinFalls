using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Backend.Effects;

/// <summary>Advances time in the game clock.</summary>
public class AdvanceTimeEffectHandler : IEffectHandler
{
    public string EffectType => "AdvanceTime";

    public void Apply(ConversationEffect effect, GameState gameState)
    {
        var parameters = effect.Parameters;

        // Try to get hours and minutes
        var hasHours = int.TryParse(parameters.GetValueOrDefault("hours", "0"), out var hours);
        var hasMinutes = int.TryParse(parameters.GetValueOrDefault("minutes", "0"), out var minutes);

        if (hasHours || hasMinutes)
        {
            gameState.Clock.AdvanceTime(hours, minutes);
        }
    }
}

