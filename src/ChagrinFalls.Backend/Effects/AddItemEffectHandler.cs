using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Backend.Effects;
/// <summary>Adds an item to the player's inventory.</summary>
public class AddItemEffectHandler : IEffectHandler
{
    public string EffectType => "AddItem";
    public void Apply(ConversationEffect effect, GameState gameState)
    {
        if (effect.Parameters.TryGetValue("itemId", out var itemId) &&
            !string.IsNullOrWhiteSpace(itemId))
            gameState.Inventory.AddItem(itemId);
    }
}
