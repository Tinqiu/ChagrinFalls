using ChagrinFalls.Backend.Conditions;
using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class ConditionEvaluatorTests
{
    // ── ItemInInventory ───────────────────────────────────────────────────────

    [Fact]
    public void ItemInInventory_ReturnsTrue_WhenItemPresent()
    {
        var state = new GameState();
        state.Inventory.AddItem("rusty_key");

        var evaluator = new ItemInInventoryEvaluator();
        var condition = new Condition
        {
            ConditionType = "ItemInInventory",
            Parameters = new Dictionary<string, string> { ["itemId"] = "rusty_key" }
        };

        Assert.True(evaluator.Evaluate(condition, state));
    }

    [Fact]
    public void ItemInInventory_ReturnsFalse_WhenItemAbsent()
    {
        var state = new GameState();
        var evaluator = new ItemInInventoryEvaluator();
        var condition = new Condition
        {
            ConditionType = "ItemInInventory",
            Parameters = new Dictionary<string, string> { ["itemId"] = "rusty_key" }
        };

        Assert.False(evaluator.Evaluate(condition, state));
    }

    [Fact]
    public void ItemInInventory_ReturnsFalse_WhenParameterMissing()
    {
        var state = new GameState();
        var evaluator = new ItemInInventoryEvaluator();
        var condition = new Condition { ConditionType = "ItemInInventory" };

        Assert.False(evaluator.Evaluate(condition, state));
    }

    // ── InformationLearned ────────────────────────────────────────────────────

    [Fact]
    public void InformationLearned_ReturnsTrue_WhenInformationKnown()
    {
        var state = new GameState();
        state.Journal.LearnInformation("victim_identity");

        var evaluator = new InformationLearnedEvaluator();
        var condition = new Condition
        {
            ConditionType = "InformationLearned",
            Parameters = new Dictionary<string, string> { ["informationId"] = "victim_identity" }
        };

        Assert.True(evaluator.Evaluate(condition, state));
    }

    [Fact]
    public void InformationLearned_ReturnsFalse_WhenInformationUnknown()
    {
        var state = new GameState();
        var evaluator = new InformationLearnedEvaluator();
        var condition = new Condition
        {
            ConditionType = "InformationLearned",
            Parameters = new Dictionary<string, string> { ["informationId"] = "victim_identity" }
        };

        Assert.False(evaluator.Evaluate(condition, state));
    }

    [Fact]
    public void InformationLearned_ReturnsFalse_WhenParameterMissing()
    {
        var state = new GameState();
        var evaluator = new InformationLearnedEvaluator();
        var condition = new Condition { ConditionType = "InformationLearned" };

        Assert.False(evaluator.Evaluate(condition, state));
    }
}
