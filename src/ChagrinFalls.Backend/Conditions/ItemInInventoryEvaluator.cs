using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Backend.Conditions;

/// <summary>
/// Evaluates the <c>ItemInInventory</c> condition.
/// Requires parameter <c>itemId</c> — the identifier of the item to check for.
/// </summary>
public class ItemInInventoryEvaluator : IConditionEvaluator
{
    public string ConditionType => "ItemInInventory";

    public bool Evaluate(Condition condition, GameState gameState)
    {
        if (!condition.Parameters.TryGetValue("itemId", out var itemId))
            return false;

        return gameState.Inventory.HasItem(itemId);
    }
}
