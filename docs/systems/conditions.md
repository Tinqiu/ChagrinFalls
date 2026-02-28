# Conditions

Conditions gate whether a `ConversationEvent`, `DialogueLine`, or `Choice` is available to the player at runtime. Every condition is represented by a `Condition` model and evaluated by an `IConditionEvaluator`.

---

## The `Condition` model

```csharp
public class Condition
{
    public string ConditionType { get; set; }
    public Dictionary<string, string> Parameters { get; set; }
}
```

`ConditionType` is matched case-insensitively against registered evaluators. `Parameters` is a free-form key-value bag interpreted by the evaluator.

---

## Built-in evaluators

| `ConditionType` | Required parameter | Description |
|---|---|---|
| `ItemInInventory` | `itemId` | True when `GameState.Inventory.HasItem(itemId)` |
| `InformationLearned` | `informationId` | True when `GameState.Journal.HasLearned(informationId)` |

### Example — gate a choice behind an item

```csharp
new Condition
{
    ConditionType = "ItemInInventory",
    Parameters    = new Dictionary<string, string> { ["itemId"] = "brass_key" }
}
```

### Example — gate a line behind learned information

```csharp
new Condition
{
    ConditionType = "InformationLearned",
    Parameters    = new Dictionary<string, string> { ["informationId"] = "victim_identity" }
}
```

---

## The `IConditionEvaluator` interface

```csharp
public interface IConditionEvaluator
{
    string ConditionType { get; }
    bool Evaluate(Condition condition, GameState gameState);
}
```

Implement this interface to create custom conditions.

---

## Registering a custom evaluator

Pass extra evaluators to the `DialogueManager` constructor, or call `RegisterEvaluator` after construction:

```csharp
public class QuestCompleteEvaluator : IConditionEvaluator
{
    public string ConditionType => "QuestComplete";

    public bool Evaluate(Condition condition, GameState gameState)
    {
        var questId = condition.Parameters["questId"];
        // Check your quest system here...
        return false;
    }
}

// Option A — via constructor
var manager = new DialogueManager(gameState, [new QuestCompleteEvaluator()]);

// Option B — after construction
manager.RegisterEvaluator(new QuestCompleteEvaluator());
```

!!! note
    If a `ConditionType` has no registered evaluator the condition is treated as **unmet**, so unknown types will silently hide content rather than throw.

