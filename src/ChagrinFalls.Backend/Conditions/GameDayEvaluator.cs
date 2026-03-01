using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Backend.Conditions;

/// <summary>
/// Evaluates conditions based on the current game day.
/// Supports checking if day is within a range (between) or at a specific day.
/// Parameters:
/// - "startDay" (required): Starting day number (1+)
/// - "endDay" (required if using between): Ending day number (1+)
/// - "mode" (optional, default "onOrAfter"): "between", "exact", or "onOrAfter"
/// </summary>
public class GameDayEvaluator : IConditionEvaluator
{
    public string ConditionType => "GameDay";

    public bool Evaluate(Condition condition, GameState gameState)
    {
        var parameters = condition.Parameters;

        if (!parameters.TryGetValue("mode", out var mode))
            mode = "onOrAfter";

        return mode.ToLowerInvariant() switch
        {
            "between" => EvaluateBetween(parameters, gameState),
            "exact" => EvaluateExact(parameters, gameState),
            "onOrAfter" => EvaluateOnOrAfter(parameters, gameState),
            _ => false
        };
    }

    private bool EvaluateBetween(Dictionary<string, string> parameters, GameState gameState)
    {
        if (!int.TryParse(parameters.GetValueOrDefault("startDay", "1"), out var startDay))
            return false;
        if (!int.TryParse(parameters.GetValueOrDefault("endDay", "1"), out var endDay))
            return false;

        var currentDay = gameState.DayTracker.CurrentDay;
        return currentDay >= startDay && currentDay <= endDay;
    }

    private bool EvaluateExact(Dictionary<string, string> parameters, GameState gameState)
    {
        if (!int.TryParse(parameters.GetValueOrDefault("startDay", "1"), out var day))
            return false;

        return gameState.DayTracker.CurrentDay == day;
    }

    private bool EvaluateOnOrAfter(Dictionary<string, string> parameters, GameState gameState)
    {
        if (!int.TryParse(parameters.GetValueOrDefault("startDay", "1"), out var day))
            return false;

        return gameState.DayTracker.CurrentDay >= day;
    }
}

