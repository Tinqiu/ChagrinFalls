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

    // negate=true cases

    [Fact]
    public void ItemInInventory_Negated_ReturnsTrue_WhenItemAbsent()
    {
        var state     = new GameState(); // item not added
        var evaluator = new ItemInInventoryEvaluator();
        var condition = new Condition
        {
            ConditionType = "ItemInInventory",
            Parameters    = new Dictionary<string, string>
                { ["itemId"] = "rusty_key", ["negate"] = "true" }
        };

        Assert.True(evaluator.Evaluate(condition, state));
    }

    [Fact]
    public void ItemInInventory_Negated_ReturnsFalse_WhenItemPresent()
    {
        var state = new GameState();
        state.Inventory.AddItem("rusty_key");
        var evaluator = new ItemInInventoryEvaluator();
        var condition = new Condition
        {
            ConditionType = "ItemInInventory",
            Parameters    = new Dictionary<string, string>
                { ["itemId"] = "rusty_key", ["negate"] = "true" }
        };

        Assert.False(evaluator.Evaluate(condition, state));
    }

    [Fact]
    public void ItemInInventory_Negated_IsCaseInsensitive_OnNegateValue()
    {
        var state = new GameState(); // item absent — negate=TRUE should still return true
        var evaluator = new ItemInInventoryEvaluator();

        foreach (var negateValue in new[] { "TRUE", "True", "tRuE" })
        {
            var condition = new Condition
            {
                ConditionType = "ItemInInventory",
                Parameters    = new Dictionary<string, string>
                    { ["itemId"] = "rusty_key", ["negate"] = negateValue }
            };
            Assert.True(evaluator.Evaluate(condition, state),
                $"Expected true for negate=\"{negateValue}\" when item is absent");
        }
    }

    [Fact]
    public void ItemInInventory_Negated_False_BehavesLikeNoNegate()
    {
        // negate=false should be identical to omitting the parameter entirely
        var state = new GameState();
        state.Inventory.AddItem("rusty_key");
        var evaluator = new ItemInInventoryEvaluator();
        var condition = new Condition
        {
            ConditionType = "ItemInInventory",
            Parameters    = new Dictionary<string, string>
                { ["itemId"] = "rusty_key", ["negate"] = "false" }
        };

        Assert.True(evaluator.Evaluate(condition, state));
    }

    [Fact]
    public void ItemInInventory_Negated_WithMissingItemId_ReturnsFalse()
    {
        // negate=true but no itemId — should still return false (missing required param)
        var state     = new GameState();
        var evaluator = new ItemInInventoryEvaluator();
        var condition = new Condition
        {
            ConditionType = "ItemInInventory",
            Parameters    = new Dictionary<string, string> { ["negate"] = "true" }
        };

        Assert.False(evaluator.Evaluate(condition, state));
    }

    // ── InformationLearned ────────────────────────────────────────────────────

    [Fact]
    public void InformationLearned_ReturnsTrue_WhenInformationKnown()
    {
        var state = new GameState();
        var entry = new JournalEntry()
        {
            Id = "victim_identity",
            Title = "Victim's Identity",
            Description = "You discovered the identity of the victim."
        };
        state.Journal.LearnInformation(entry);

        var evaluator = new InformationLearnedEvaluator();
        var condition = new Condition
        {
            ConditionType = "InformationLearned",
            Parameters = new Dictionary<string, string> { ["informationId"] = entry.Id }
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
