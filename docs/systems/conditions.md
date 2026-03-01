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
| `TimeOfDay` | `startHour`, `endHour` | True when current time falls within a range or matches exact time |
| `GameDay` | `startDay`, `endDay` | True when current day falls within a range, equals a day, or is on/after a day |

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

### Example — gate content to afternoon/evening (12:00–20:00)

```csharp
new Condition
{
    ConditionType = "TimeOfDay",
    Parameters = new Dictionary<string, string>
    {
        ["startHour"] = "12",
        ["startMinute"] = "0",
        ["endHour"] = "20",
        ["endMinute"] = "0",
        ["mode"] = "between"
    }
}
```

### Example — gate content to night hours (22:00–06:00, wraps midnight)

```csharp
new Condition
{
    ConditionType = "TimeOfDay",
    Parameters = new Dictionary<string, string>
    {
        ["startHour"] = "22",
        ["endHour"] = "6",
        ["mode"] = "between"
    }
}
```

### Example — gate content to a specific time (exact match)

```csharp
new Condition
{
    ConditionType = "TimeOfDay",
    Parameters = new Dictionary<string, string>
    {
        ["startHour"] = "14",
        ["startMinute"] = "30",
        ["mode"] = "exact"
    }
}
```

### Example — gate content to days 2–4

```csharp
new Condition
{
    ConditionType = "GameDay",
    Parameters = new Dictionary<string, string>
    {
        ["startDay"] = "2",
        ["endDay"] = "4",
        ["mode"] = "between"
    }
}
```

### Example — gate content to day 5 only

```csharp
new Condition
{
    ConditionType = "GameDay",
    Parameters = new Dictionary<string, string>
    {
        ["startDay"] = "5",
        ["mode"] = "exact"
    }
}
```

### Example — gate content to day 3 or later

```csharp
new Condition
{
    ConditionType = "GameDay",
    Parameters = new Dictionary<string, string>
    {
        ["startDay"] = "3",
        ["mode"] = "onOrAfter"
    }
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

