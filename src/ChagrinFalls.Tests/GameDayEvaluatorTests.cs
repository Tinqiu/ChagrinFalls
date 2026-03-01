using ChagrinFalls.Backend.Conditions;
using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class GameDayEvaluatorTests
{
    [Fact]
    public void Between_ReturnsTrue_WhenDayIsWithinRange()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(3);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "1" },
                { "endDay", "5" },
                { "mode", "between" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Between_ReturnsTrue_WhenDayIsAtStartBoundary()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(1);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "1" },
                { "endDay", "5" },
                { "mode", "between" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Between_ReturnsTrue_WhenDayIsAtEndBoundary()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(5);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "1" },
                { "endDay", "5" },
                { "mode", "between" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Between_ReturnsFalse_WhenDayIsBeforeStart()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(0); // This would fail in constructor, so use 1 and test logic
        gameState.DayTracker.SetDay(1);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "2" },
                { "endDay", "5" },
                { "mode", "between" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.False(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Between_ReturnsFalse_WhenDayIsAfterEnd()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(10);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "1" },
                { "endDay", "5" },
                { "mode", "between" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.False(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Exact_ReturnsTrue_WhenDayMatches()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(5);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "5" },
                { "mode", "exact" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void Exact_ReturnsFalse_WhenDayDoesNotMatch()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(5);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "4" },
                { "mode", "exact" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.False(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void OnOrAfter_ReturnsTrue_WhenDayIsAfter()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(10);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "5" },
                { "mode", "onOrAfter" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void OnOrAfter_ReturnsTrue_WhenDayIsEqual()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(5);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "5" },
                { "mode", "onOrAfter" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void OnOrAfter_ReturnsFalse_WhenDayIsBefore()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(3);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "5" },
                { "mode", "onOrAfter" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.False(evaluator.Evaluate(condition, gameState));
    }

    [Fact]
    public void DefaultMode_IsOnOrAfter()
    {
        var gameState = new GameState();
        gameState.DayTracker.SetDay(10);

        var condition = new Condition
        {
            ConditionType = "GameDay",
            Parameters = new Dictionary<string, string>
            {
                { "startDay", "5" }
            }
        };

        var evaluator = new GameDayEvaluator();
        Assert.True(evaluator.Evaluate(condition, gameState));
    }
}


