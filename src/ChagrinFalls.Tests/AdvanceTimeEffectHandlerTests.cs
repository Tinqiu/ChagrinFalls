using ChagrinFalls.Backend.Effects;
using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class AdvanceTimeEffectHandlerTests
{
    [Fact]
    public void Apply_AdvancesClockByHours()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(10, 0);

        var effect = new ConversationEffect
        {
            EffectType = "AdvanceTime",
            Parameters = new Dictionary<string, string> { { "hours", "3" } }
        };

        var handler = new AdvanceTimeEffectHandler();
        handler.Apply(effect, gameState);

        Assert.Equal(13, gameState.Clock.Hour);
        Assert.Equal(0, gameState.Clock.Minute);
    }

    [Fact]
    public void Apply_AdvancesClockByMinutes()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(10, 0);

        var effect = new ConversationEffect
        {
            EffectType = "AdvanceTime",
            Parameters = new Dictionary<string, string> { { "minutes", "45" } }
        };

        var handler = new AdvanceTimeEffectHandler();
        handler.Apply(effect, gameState);

        Assert.Equal(10, gameState.Clock.Hour);
        Assert.Equal(45, gameState.Clock.Minute);
    }

    [Fact]
    public void Apply_AdvancesClockByHoursAndMinutes()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(10, 30);

        var effect = new ConversationEffect
        {
            EffectType = "AdvanceTime",
            Parameters = new Dictionary<string, string>
            {
                { "hours", "2" },
                { "minutes", "45" }
            }
        };

        var handler = new AdvanceTimeEffectHandler();
        handler.Apply(effect, gameState);

        Assert.Equal(13, gameState.Clock.Hour);
        Assert.Equal(15, gameState.Clock.Minute);
    }

    [Fact]
    public void Apply_WrapsAroundMidnight()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(22, 0);

        var effect = new ConversationEffect
        {
            EffectType = "AdvanceTime",
            Parameters = new Dictionary<string, string> { { "hours", "3" } }
        };

        var handler = new AdvanceTimeEffectHandler();
        handler.Apply(effect, gameState);

        Assert.Equal(1, gameState.Clock.Hour);
        Assert.Equal(0, gameState.Clock.Minute);
    }

    [Fact]
    public void Apply_IgnoresInvalidParameters()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(10, 0);

        var effect = new ConversationEffect
        {
            EffectType = "AdvanceTime",
            Parameters = new Dictionary<string, string> { { "hours", "invalid" } }
        };

        var handler = new AdvanceTimeEffectHandler();
        handler.Apply(effect, gameState); // Should not throw

        Assert.Equal(10, gameState.Clock.Hour);
        Assert.Equal(0, gameState.Clock.Minute);
    }

    [Fact]
    public void Apply_HandlesEmptyParameters()
    {
        var gameState = new GameState();
        gameState.Clock.SetTime(10, 0);

        var effect = new ConversationEffect
        {
            EffectType = "AdvanceTime",
            Parameters = new Dictionary<string, string>()
        };

        var handler = new AdvanceTimeEffectHandler();
        handler.Apply(effect, gameState); // Should not throw

        Assert.Equal(10, gameState.Clock.Hour);
        Assert.Equal(0, gameState.Clock.Minute);
    }
}

