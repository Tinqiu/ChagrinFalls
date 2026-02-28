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

        var hasItem = gameState.Inventory.HasItem(itemId);
        var negate  = condition.Parameters.TryGetValue("negate", out var negVal)
                      && negVal.Equals("true", StringComparison.OrdinalIgnoreCase);
        return negate ? !hasItem : hasItem;
    }
}
