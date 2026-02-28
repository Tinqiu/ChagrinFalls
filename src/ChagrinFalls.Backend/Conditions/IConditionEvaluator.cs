using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Backend.Conditions;

/// <summary>
/// Evaluates whether a <see cref="Condition"/> is satisfied given the current <see cref="GameState"/>.
/// </summary>
public interface IConditionEvaluator
{
    /// <summary>
    /// The condition type string this evaluator handles (e.g. "ItemInInventory").
    /// </summary>
    string ConditionType { get; }

    /// <summary>
    /// Returns <c>true</c> when the condition is satisfied; <c>false</c> otherwise.
    /// </summary>
    bool Evaluate(Condition condition, GameState gameState);
}
