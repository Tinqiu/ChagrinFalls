using ChagrinFalls.Backend.Conditions;
using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class TimeOfDayEvaluatorTests
{
    [Fact]
    public void Between_ReturnsTrue_WhenTimeIsWithinRange()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(14, 30);

        var condition = new Condition
        {
            ConditionType = "TimeOfDay",
            Parameters = new Dictionary<string, string>
            {
                { "startHour", "10" },
                { "startMinute", "0" },
                { "endHour", "18" },
                { "endMinute", "0" },
                { "mode", "between" }
            }
        };

        var evaluator = new TimeOfDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Between_ReturnsFalse_WhenTimeIsOutsideRange()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(8, 0);

        var condition = new Condition
        {
            ConditionType = "TimeOfDay",
            Parameters = new Dictionary<string, string>
            {
                { "startHour", "10" },
                { "endHour", "18" },
                { "mode", "between" }
            }
        };

        var evaluator = new TimeOfDayEvaluator();
        Assert.False(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Between_SupportsWrappingAroundMidnight()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(23, 30);

        var condition = new Condition
        {
            ConditionType = "TimeOfDay",
            Parameters = new Dictionary<string, string>
            {
                { "startHour", "22" },
                { "endHour", "6" },
                { "mode", "between" }
            }
        };

        var evaluator = new TimeOfDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Between_SupportsWrappingAroundMidnight_EarlyMorning()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(4, 0);

        var condition = new Condition
        {
            ConditionType = "TimeOfDay",
            Parameters = new Dictionary<string, string>
            {
                { "startHour", "22" },
                { "endHour", "6" },
                { "mode", "between" }
            }
        };

        var evaluator = new TimeOfDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Between_ReturnsFalse_WhenOutsideWrappingRange()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(12, 0);

        var condition = new Condition
        {
            ConditionType = "TimeOfDay",
            Parameters = new Dictionary<string, string>
            {
                { "startHour", "22" },
                { "endHour", "6" },
                { "mode", "between" }
            }
        };

        var evaluator = new TimeOfDayEvaluator();
        Assert.False(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Exact_ReturnsTrue_WhenTimeMatches()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(14, 30);

        var condition = new Condition
        {
            ConditionType = "TimeOfDay",
            Parameters = new Dictionary<string, string>
            {
                { "startHour", "14" },
                { "startMinute", "30" },
                { "mode", "exact" }
            }
        };

        var evaluator = new TimeOfDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Exact_ReturnsFalse_WhenTimeDoesNotMatch()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(14, 31);

        var condition = new Condition
        {
            ConditionType = "TimeOfDay",
            Parameters = new Dictionary<string, string>
            {
                { "startHour", "14" },
                { "startMinute", "30" },
                { "mode", "exact" }
            }
        };

        var evaluator = new TimeOfDayEvaluator();
        Assert.False(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void DefaultMode_IsBetween()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(14, 30);

        var condition = new Condition
        {
            ConditionType = "TimeOfDay",
            Parameters = new Dictionary<string, string>
            {
                { "startHour", "10" },
                { "endHour", "18" }
            }
        };

        var evaluator = new TimeOfDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void DefaultMinutes_AreZero()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(10, 0);

        var condition = new Condition
        {
            ConditionType = "TimeOfDay",
            Parameters = new Dictionary<string, string>
            {
                { "startHour", "10" },
                { "endHour", "18" }
            }
        };

        var evaluator = new TimeOfDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }
}

