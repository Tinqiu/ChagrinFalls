# Dialogue Manager

`ChagrinFalls.Backend.Systems.DialogueManager`

The `DialogueManager` is the core runtime engine for conversations. It owns a single active `ConversationEvent`, advances through `DialogueLine`s, evaluates conditions, and surfaces available player choices.

---

## Constructor

```csharp
public DialogueManager(GameState gameState, IEnumerable<IConditionEvaluator>? evaluators = null)
```

| Parameter | Description |
|---|---|
| `gameState` | The current game state used when evaluating conditions. |
| `evaluators` | Optional additional condition evaluators. The built-in `ItemInInventory` and `InformationLearned` evaluators are always registered automatically. |

---

## Properties

### `CurrentLine`
```csharp
public DialogueLine? CurrentLine { get; }
```
The dialogue line currently being displayed, or `null` when no conversation is active.

### `AvailableChoices`
```csharp
public IReadOnlyList<Choice> AvailableChoices { get; }
```
The player choices for the current line, filtered to those whose conditions are met. Empty when no conversation is active or the line has no choices.

### `IsConversationActive`
```csharp
public bool IsConversationActive { get; }
```
`true` while a conversation is in progress.

---

## Events

### `OnDialogueLineChanged`
```csharp
public event Action<DialogueLine>? OnDialogueLineChanged;
```
Raised whenever the active dialogue line changes. Subscribe to this to update the UI.

### `OnConversationEnded`
```csharp
public event Action? OnConversationEnded;
```
Raised when the conversation ends — no more lines or choices remain.

---

## Methods

### `StartEvent`
```csharp
public void StartEvent(ConversationEvent conversationEvent)
```
Starts the given conversation event, beginning with its `StartingConversationId`. Throws `InvalidOperationException` if the event's conditions are not met or required conversations/lines are missing.

### `Advance`
```csharp
public void Advance()
```
Moves to the next dialogue line when no choices are present. If the current line has choices, this is a no-op — use `SelectChoice` instead.

### `SelectChoice`
```csharp
public void SelectChoice(int choiceIndex)
```
Selects a player choice by its zero-based index into `AvailableChoices` and advances to the line it points to. Throws `ArgumentOutOfRangeException` if the index is invalid.

### `RegisterEvaluator`
```csharp
public void RegisterEvaluator(IConditionEvaluator evaluator)
```
Registers or replaces a condition evaluator. See the [Conditions](conditions.md) page for details.

---

## Example

```csharp
var state   = new GameState();
var manager = new DialogueManager(state);

manager.OnDialogueLineChanged += line =>
    Console.WriteLine($"{line.Speaker}: {line.Text}");

manager.OnConversationEnded += () =>
    Console.WriteLine("Conversation over.");

manager.StartEvent(myConversationEvent);

// Advance through lines with no choices
while (manager.IsConversationActive && manager.AvailableChoices.Count == 0)
    manager.Advance();
```

