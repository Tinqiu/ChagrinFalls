using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Backend.Effects;
/// <summary>Removes an item from the player's inventory.</summary>
public class RemoveItemEffectHandler : IEffectHandler
{
    public string EffectType => "RemoveItem";
    public void Apply(ConversationEffect effect, GameState gameState)
    {
        if (effect.Parameters.TryGetValue("itemId", out var itemId) &&
            !string.IsNullOrWhiteSpace(itemId))
            gameState.Inventory.RemoveItem(itemId);
    }
}
