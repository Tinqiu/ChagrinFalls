using ChagrinFalls.Backend.Conditions;
using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class DialogueManagerTests
{
    private static DialogueManager CreateManager(GameState? state = null) =>
        new(state ?? new GameState());

    // ── StartEvent ────────────────────────────────────────────────────────────

    [Fact]
    public void StartEvent_SetsCurrentLine_ToStartingLine()
    {
        var manager = CreateManager();
        manager.StartEvent(ConversationFixtures.SimpleLinearEvent());

        Assert.NotNull(manager.CurrentLine);
        Assert.Equal("line_1", manager.CurrentLine!.Id);
    }

    [Fact]
    public void StartEvent_Throws_WhenEventConditionsNotMet()
    {
        var manager = CreateManager();
        var evt = ConversationFixtures.SimpleLinearEvent();
        evt.Conditions.Add(new Condition
        {
            ConditionType = "ItemInInventory",
            Parameters = new Dictionary<string, string> { ["itemId"] = "magic_key" }
        });

        Assert.Throws<InvalidOperationException>(() => manager.StartEvent(evt));
    }

    [Fact]
    public void StartEvent_Succeeds_WhenEventConditionsMet()
    {
        var state = new GameState();
        state.Inventory.AddItem("magic_key");
        var manager = CreateManager(state);

        var evt = ConversationFixtures.SimpleLinearEvent();
        evt.Conditions.Add(new Condition
        {
            ConditionType = "ItemInInventory",
            Parameters = new Dictionary<string, string> { ["itemId"] = "magic_key" }
        });

        manager.StartEvent(evt);
        Assert.NotNull(manager.CurrentLine);
    }

    // ── Advance ───────────────────────────────────────────────────────────────

    [Fact]
    public void Advance_MovesToNextLine()
    {
        var manager = CreateManager();
        manager.StartEvent(ConversationFixtures.SimpleLinearEvent());

        manager.Advance();

        Assert.Equal("line_2", manager.CurrentLine!.Id);
    }

    [Fact]
    public void Advance_EndsConversation_WhenNoNextLine()
    {
        var manager = CreateManager();
        manager.StartEvent(ConversationFixtures.SimpleLinearEvent());
        manager.Advance(); // line_1 → line_2
        manager.Advance(); // line_2 → end

        Assert.False(manager.IsConversationActive);
        Assert.Null(manager.CurrentLine);
    }

    [Fact]
    public void Advance_DoesNothing_WhenLineHasChoices()
    {
        var manager = CreateManager();
        manager.StartEvent(ConversationFixtures.BranchingEvent());

        manager.Advance(); // should be ignored

        Assert.Equal("line_1", manager.CurrentLine!.Id);
    }

    [Fact]
    public void Advance_DoesNothing_WhenNotActive()
    {
        var manager = CreateManager();
        manager.Advance(); // should not throw
        Assert.False(manager.IsConversationActive);
    }

    // ── SelectChoice ──────────────────────────────────────────────────────────

    [Fact]
    public void SelectChoice_NavigatesToCorrectBranch_ForChoiceA()
    {
        var manager = CreateManager();
        manager.StartEvent(ConversationFixtures.BranchingEvent());

        manager.SelectChoice(0); // choice_a → line_2a

        Assert.Equal("line_2a", manager.CurrentLine!.Id);
    }

    [Fact]
    public void SelectChoice_NavigatesToCorrectBranch_ForChoiceB()
    {
        var manager = CreateManager();
        manager.StartEvent(ConversationFixtures.BranchingEvent());

        manager.SelectChoice(1); // choice_b → line_2b

        Assert.Equal("line_2b", manager.CurrentLine!.Id);
    }

    [Fact]
    public void SelectChoice_Throws_WhenIndexOutOfRange()
    {
        var manager = CreateManager();
        manager.StartEvent(ConversationFixtures.BranchingEvent());

        Assert.Throws<ArgumentOutOfRangeException>(() => manager.SelectChoice(5));
    }

    // ── Condition filtering on choices ────────────────────────────────────────

    [Fact]
    public void AvailableChoices_ExcludesChoicesWhoseConditionsAreNotMet()
    {
        var manager = CreateManager(); // no items in inventory

        var evt = ConversationFixtures.BranchingEvent();
        // Add a condition to choice_a that won't be satisfied
        evt.Conversations["conv_1"].DialogueLines["line_1"].Choices[0].Conditions.Add(
            new Condition
            {
                ConditionType = "ItemInInventory",
                Parameters = new Dictionary<string, string> { ["itemId"] = "rare_gem" }
            });

        manager.StartEvent(evt);

        Assert.Single(manager.AvailableChoices);
        Assert.Equal("choice_b", manager.AvailableChoices[0].Id);
    }

    [Fact]
    public void AvailableChoices_IncludesChoice_WhenItsConditionIsMet()
    {
        var state = new GameState();
        state.Inventory.AddItem("rare_gem");
        var manager = CreateManager(state);

        var evt = ConversationFixtures.BranchingEvent();
        evt.Conversations["conv_1"].DialogueLines["line_1"].Choices[0].Conditions.Add(
            new Condition
            {
                ConditionType = "ItemInInventory",
                Parameters = new Dictionary<string, string> { ["itemId"] = "rare_gem" }
            });

        manager.StartEvent(evt);

        Assert.Equal(2, manager.AvailableChoices.Count);
    }

    // ── Condition-gated dialogue lines ────────────────────────────────────────

    [Fact]
    public void Advance_SkipsLine_WhenItsConditionIsNotMet()
    {
        var manager = CreateManager(); // no items in inventory

        // line_1 → line_2 (gated by item) → line_3 (always shown)
        var line3 = new DialogueLine { Id = "line_3", Speaker = "NPC", Text = "Always shown." };
        var line2 = new DialogueLine
        {
            Id = "line_2",
            Speaker = "NPC",
            Text = "Only with item.",
            NextDialogueLineId = "line_3",
            Conditions = new List<Condition>
            {
                new()
                {
                    ConditionType = "ItemInInventory",
                    Parameters = new Dictionary<string, string> { ["itemId"] = "magic_key" }
                }
            }
        };
        var line1 = new DialogueLine
        {
            Id = "line_1",
            Speaker = "NPC",
            Text = "Start.",
            NextDialogueLineId = "line_2"
        };

        var conversation = new Conversation
        {
            Id = "conv_1",
            StartingDialogueLineId = "line_1",
            DialogueLines = new Dictionary<string, DialogueLine>
            {
                [line1.Id] = line1,
                [line2.Id] = line2,
                [line3.Id] = line3
            }
        };

        var evt = new ConversationEvent
        {
            Id = "gated_event",
            StartingConversationId = "conv_1",
            Conversations = new Dictionary<string, Conversation> { [conversation.Id] = conversation }
        };

        manager.StartEvent(evt);
        manager.Advance(); // should skip line_2 (condition not met) and land on line_3

        Assert.Equal("line_3", manager.CurrentLine!.Id);
    }

    // ── Events ────────────────────────────────────────────────────────────────

    [Fact]
    public void OnDialogueLineChanged_FiredWhenLineChanges()
    {
        var manager = CreateManager();
        var firedLines = new List<string>();
        manager.OnDialogueLineChanged += line => firedLines.Add(line.Id);

        manager.StartEvent(ConversationFixtures.SimpleLinearEvent());
        manager.Advance();

        Assert.Equal(new[] { "line_1", "line_2" }, firedLines);
    }

    [Fact]
    public void OnConversationEnded_FiredWhenConversationEnds()
    {
        var manager = CreateManager();
        var ended = false;
        manager.OnConversationEnded += () => ended = true;

        manager.StartEvent(ConversationFixtures.SimpleLinearEvent());
        manager.Advance(); // line_1 → line_2
        manager.Advance(); // line_2 → end

        Assert.True(ended);
    }

    // ── Custom evaluators ─────────────────────────────────────────────────────

    [Fact]
    public void RegisterEvaluator_AllowsCustomConditionType()
    {
        var manager = CreateManager();

        // Custom evaluator that always returns true
        var custom = new AlwaysTrueEvaluator();
        manager.RegisterEvaluator(custom);

        var evt = ConversationFixtures.SimpleLinearEvent();
        evt.Conditions.Add(new Condition { ConditionType = "AlwaysTrue" });

        manager.StartEvent(evt);
        Assert.NotNull(manager.CurrentLine);
    }

    private sealed class AlwaysTrueEvaluator : IConditionEvaluator
    {
        public string ConditionType => "AlwaysTrue";
        public bool Evaluate(Condition condition, GameState gameState) => true;
    }
}
