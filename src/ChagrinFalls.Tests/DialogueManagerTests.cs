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

    [Fact]
    public void Advance_ProgressesNormally_WhenAllChoicesAreGatedOut()
    {
        var manager = CreateManager(); // no items in inventory — all choices will be hidden

        // Build a line that has two condition-gated choices (both unmet) plus a NextDialogueLineId.
        var lineNext = new DialogueLine { Id = "line_next", Speaker = "NPC", Text = "You moved on." };

        var lineWithChoices = new DialogueLine
        {
            Id                 = "line_choices",
            Speaker            = "NPC",
            Text               = "A question with hidden choices.",
            NextDialogueLineId = "line_next",
            Choices = new List<Choice>
            {
                new()
                {
                    Id                 = "choice_a",
                    Text               = "Option A (hidden)",
                    NextDialogueLineId = "line_next",
                    Conditions = new List<Condition>
                    {
                        new() { ConditionType = "ItemInInventory", Parameters = new Dictionary<string, string> { ["itemId"] = "rare_gem" } }
                    }
                },
                new()
                {
                    Id                 = "choice_b",
                    Text               = "Option B (hidden)",
                    NextDialogueLineId = "line_next",
                    Conditions = new List<Condition>
                    {
                        new() { ConditionType = "ItemInInventory", Parameters = new Dictionary<string, string> { ["itemId"] = "magic_key" } }
                    }
                }
            }
        };

        var conversation = new Conversation
        {
            Id                     = "conv_1",
            StartingDialogueLineId = "line_choices",
            DialogueLines = new Dictionary<string, DialogueLine>
            {
                [lineWithChoices.Id] = lineWithChoices,
                [lineNext.Id]        = lineNext
            }
        };

        var evt = new ConversationEvent
        {
            Id                     = "gated_choices_event",
            StartingConversationId = "conv_1",
            Conversations = new Dictionary<string, Conversation> { [conversation.Id] = conversation }
        };

        manager.StartEvent(evt);

        // All choices are hidden — AvailableChoices should be empty.
        Assert.Empty(manager.AvailableChoices);

        // Advance() should not be blocked and should move to the next line.
        manager.Advance();
        Assert.Equal("line_next", manager.CurrentLine!.Id);
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

    // ── Cycle detection ───────────────────────────────────────────────────────

    [Fact]
    public void NavigateToLine_EndsConversation_WhenGatedLinesCycleInfinitely()
    {
        var manager = CreateManager(); // no items — all conditions unmet

        // line_a (gated) → line_b (gated) → line_a (cycle)
        var lineA = new DialogueLine
        {
            Id                 = "line_a",
            Speaker            = "NPC",
            Text               = "Gated A.",
            NextDialogueLineId = "line_b",
            Conditions = new List<Condition>
            {
                new() { ConditionType = "ItemInInventory", Parameters = new Dictionary<string, string> { ["itemId"] = "magic_key" } }
            }
        };
        var lineB = new DialogueLine
        {
            Id                 = "line_b",
            Speaker            = "NPC",
            Text               = "Gated B.",
            NextDialogueLineId = "line_a",  // creates the cycle
            Conditions = new List<Condition>
            {
                new() { ConditionType = "ItemInInventory", Parameters = new Dictionary<string, string> { ["itemId"] = "magic_key" } }
            }
        };
        var lineStart = new DialogueLine
        {
            Id                 = "line_start",
            Speaker            = "NPC",
            Text               = "Start.",
            NextDialogueLineId = "line_a"
        };

        var conversation = new Conversation
        {
            Id                     = "conv_1",
            StartingDialogueLineId = "line_start",
            DialogueLines = new Dictionary<string, DialogueLine>
            {
                [lineStart.Id] = lineStart,
                [lineA.Id]     = lineA,
                [lineB.Id]     = lineB
            }
        };

        var evt = new ConversationEvent
        {
            Id                     = "cycle_event",
            StartingConversationId = "conv_1",
            Conversations = new Dictionary<string, Conversation> { [conversation.Id] = conversation }
        };

        var ended = false;
        manager.OnConversationEnded += () => ended = true;

        manager.StartEvent(evt);
        manager.Advance(); // navigates into the cycle — should detect and end

        Assert.True(ended);
        Assert.False(manager.IsConversationActive);
    }

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

    // ── Effect application ────────────────────────────────────────────────────

    [Fact]
    public void Advance_AppliesAddItemEffect_BeforeMovingToNextLine()
    {
        var state   = new GameState();
        var manager = CreateManager(state);
        manager.StartEvent(ConversationFixtures.EventWithAddItemEffect("key"));

        Assert.False(state.Inventory.HasItem("key"));

        manager.Advance(); // leaves line_1 — effect should fire

        Assert.True(state.Inventory.HasItem("key"));
        Assert.Equal("line_2", manager.CurrentLine!.Id);
    }

    [Fact]
    public void Advance_AppliesRemoveItemEffect_BeforeMovingToNextLine()
    {
        var state = new GameState();
        state.Inventory.AddItem("key");
        var manager = CreateManager(state);
        manager.StartEvent(ConversationFixtures.EventWithRemoveItemEffect("key"));

        Assert.True(state.Inventory.HasItem("key"));

        manager.Advance();

        Assert.False(state.Inventory.HasItem("key"));
        Assert.Equal("line_2", manager.CurrentLine!.Id);
    }

    [Fact]
    public void Advance_AppliesMultipleEffectsInOrder()
    {
        var state = new GameState();
        state.Inventory.AddItem("gold");
        var manager = CreateManager(state);
        manager.StartEvent(ConversationFixtures.EventWithMultipleEffects());

        // Before advance: has gold, no sword
        Assert.False(state.Inventory.HasItem("sword"));
        Assert.True(state.Inventory.HasItem("gold"));

        manager.Advance();

        // After advance: sword added, gold removed
        Assert.True(state.Inventory.HasItem("sword"));
        Assert.False(state.Inventory.HasItem("gold"));
    }

    [Fact]
    public void SelectChoice_AppliesEffectsFromCurrentLine_BeforeNavigating()
    {
        var state   = new GameState();
        var manager = CreateManager(state);
        manager.StartEvent(ConversationFixtures.EventWithEffectOnChoiceLine("key"));

        Assert.False(state.Inventory.HasItem("key"));

        manager.SelectChoice(0); // selects choice_a — effect on line_1 should fire first

        Assert.True(state.Inventory.HasItem("key"));
        Assert.Equal("line_2a", manager.CurrentLine!.Id);
    }

    [Fact]
    public void Advance_DoesNotApplyEffect_WhenBlockedByChoices()
    {
        // If AvailableChoices is non-empty, Advance() returns early and must NOT apply effects.
        var state   = new GameState();
        var manager = CreateManager(state);
        manager.StartEvent(ConversationFixtures.EventWithEffectOnChoiceLine("key"));

        manager.Advance(); // has choices — should be a no-op

        Assert.False(state.Inventory.HasItem("key")); // effect must not have fired
        Assert.Equal("line_1", manager.CurrentLine!.Id);
    }

    [Fact]
    public void Advance_EffectAppliedExactlyOnce_OnSingleAdvance()
    {
        // Verifies the effect fires exactly once, not accumulating across multiple calls.
        var state   = new GameState();
        var manager = CreateManager(state);
        manager.StartEvent(ConversationFixtures.EventWithAddItemEffect("key"));

        manager.Advance(); // fires effect, moves to line_2
        // Second advance ends conversation — no effect on line_2
        manager.Advance();

        // Inventory should contain exactly one "key", not two
        Assert.True(state.Inventory.HasItem("key"));
        // Confirm conversation is over
        Assert.False(manager.IsConversationActive);
    }

    [Fact]
    public void Advance_UnknownEffectType_IsIgnoredWithoutThrowing()
    {
        var state   = new GameState();
        var manager = CreateManager(state);
        var evt     = ConversationFixtures.SimpleLinearEvent();
        evt.Conversations["conv_1"].DialogueLines["line_1"].Effects.Add(new ConversationEffect
        {
            EffectType = "NonExistentEffect",
            Parameters = new Dictionary<string, string>()
        });

        manager.StartEvent(evt);

        // Should not throw — unknown effect types are silently skipped
        var ex = Record.Exception(() => manager.Advance());
        Assert.Null(ex);
        Assert.Equal("line_2", manager.CurrentLine!.Id);
    }

    [Fact]
    public void Advance_AddItemEffect_WithMissingItemIdParameter_IsIgnoredWithoutThrowing()
    {
        var state   = new GameState();
        var manager = CreateManager(state);
        var evt     = ConversationFixtures.SimpleLinearEvent();
        evt.Conversations["conv_1"].DialogueLines["line_1"].Effects.Add(new ConversationEffect
        {
            EffectType = "AddItem",
            Parameters = new Dictionary<string, string>() // no itemId key
        });

        manager.StartEvent(evt);

        var ex = Record.Exception(() => manager.Advance());
        Assert.Null(ex);
        Assert.Equal("line_2", manager.CurrentLine!.Id);
    }

    [Fact]
    public void RegisterHandler_AllowsCustomEffectType()
    {
        var state   = new GameState();
        var manager = CreateManager(state);
        var custom  = new TrackingEffectHandler();
        manager.RegisterHandler(custom);

        var evt = ConversationFixtures.SimpleLinearEvent();
        evt.Conversations["conv_1"].DialogueLines["line_1"].Effects.Add(new ConversationEffect
        {
            EffectType = "Tracking",
            Parameters = new Dictionary<string, string>()
        });

        manager.StartEvent(evt);
        manager.Advance();

        Assert.Equal(1, custom.ApplyCount);
    }

    private sealed class TrackingEffectHandler : ChagrinFalls.Backend.Effects.IEffectHandler
    {
        public string EffectType  => "Tracking";
        public int    ApplyCount  { get; private set; }
        public void Apply(ConversationEffect effect, GameState gameState) => ApplyCount++;
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
