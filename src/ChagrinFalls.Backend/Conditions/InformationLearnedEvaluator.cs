using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Backend.Conditions;

/// <summary>
/// Evaluates the <c>InformationLearned</c> condition.
/// Requires parameter <c>informationId</c> — the identifier of the information entry to check for.
/// </summary>
public class InformationLearnedEvaluator : IConditionEvaluator
{
    public string ConditionType => "InformationLearned";

    public bool Evaluate(Condition condition, GameState gameState)
    {
        if (!condition.Parameters.TryGetValue("informationId", out var informationId))
            return false;

        return gameState.Journal.HasLearned(informationId);
    }
}
