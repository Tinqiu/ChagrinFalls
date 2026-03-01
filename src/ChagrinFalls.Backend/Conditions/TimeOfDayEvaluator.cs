using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Backend.Conditions;

/// <summary>
/// Evaluates conditions based on the current time of day.
/// Supports checking if time is within a range (between) or at a specific time.
/// Parameters:
/// - "startHour" (required): Starting hour (0-23)
/// - "startMinute" (optional, default 0): Starting minute (0-59)
/// - "endHour" (required if using between): Ending hour (0-23)
/// - "endMinute" (optional, default 0): Ending minute (0-59)
/// - "mode" (optional, default "between"): "between" or "exact"
/// </summary>
public class TimeOfDayEvaluator : IConditionEvaluator
{
    public string ConditionType => "TimeOfDay";

    public bool Evaluate(Condition condition, GameState gameState)
    {
        var parameters = condition.Parameters;

        if (!parameters.TryGetValue("mode", out var mode))
            mode = "between";

        return mode.ToLowerInvariant() switch
        {
            "between" => EvaluateBetween(parameters, gameState),
            "exact" => EvaluateExact(parameters, gameState),
            _ => false
        };
    }

    private bool EvaluateBetween(Dictionary<string, string> parameters, GameState gameState)
    {
        if (!int.TryParse(parameters.GetValueOrDefault("startHour", "0"), out var startHour))
            return false;
        if (!int.TryParse(parameters.GetValueOrDefault("startMinute", "0"), out var startMinute))
            return false;
        if (!int.TryParse(parameters.GetValueOrDefault("endHour", "0"), out var endHour))
            return false;
        if (!int.TryParse(parameters.GetValueOrDefault("endMinute", "0"), out var endMinute))
            return false;

        var clock = gameState.Clock;
        var currentMinutes = clock.MinutesFromMidnight;
        var startMinutes = startHour * 60 + startMinute;
        var endMinutes = endHour * 60 + endMinute;

        // If end time is less than start time, the range wraps around midnight
        // (e.g., 22:00 to 06:00)
        if (endMinutes < startMinutes)
        {
            return currentMinutes >= startMinutes || currentMinutes < endMinutes;
        }

        return currentMinutes >= startMinutes && currentMinutes < endMinutes;
    }

    private bool EvaluateExact(Dictionary<string, string> parameters, GameState gameState)
    {
        if (!int.TryParse(parameters.GetValueOrDefault("startHour", "0"), out var hour))
            return false;
        if (!int.TryParse(parameters.GetValueOrDefault("startMinute", "0"), out var minute))
            return false;

        var clock = gameState.Clock;
        return clock.Hour == hour && clock.Minute == minute;
    }
}

