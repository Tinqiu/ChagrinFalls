# ChagrinFalls

A text-based mystery game built with a C# backend and a [Godot 4](https://godotengine.org/) frontend.

## Project Structure

```
ChagrinFalls/
├── src/
│   ├── ChagrinFalls.Backend/     # Pure C# class library — models, systems, dialogue engine
│   └── ChagrinFalls.Tests/       # xUnit tests for the backend
└── godot/
    └── ChagrinFalls.Godot/       # Godot 4.6 project with C# UI scripts
```

### Backend (`ChagrinFalls.Backend`)

| Namespace | Contents |
|-----------|----------|
| `Models` | `ConversationEvent`, `Conversation`, `DialogueLine`, `Choice`, `Condition`, `Participant` |
| `Systems` | `DialogueManager`, `GameState`, `PlayerJournal`, `PlayerInventory` |
| `Conditions` | `IConditionEvaluator`, `ItemInInventoryEvaluator`, `InformationLearnedEvaluator` |

### Godot project (`ChagrinFalls.Godot`)

| File | Purpose |
|------|---------|
| `Scripts/Main.cs` | Root scene — bootstraps game state and kicks off a sample conversation |
| `Scripts/DialogueUI.cs` | CanvasLayer that renders speaker name, dialogue text, and choice buttons |
| `Scenes/Main.tscn` | Root scene — dark background + DialogueUI overlay |
| `Scenes/DialogueUI.tscn` | Panel anchored to the bottom third of the screen |

## Getting Started

### Prerequisites

| Tool | Version |
|------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 9.x |
| [Godot Engine (C# / .NET build)](https://godotengine.org/download) | 4.6.1 |

> **Note:** Make sure you download the **.NET** variant of Godot 4 — the standard build does not include C# support.

---

### 1 — Clone the repository

```bash
git clone https://github.com/Tinqiu/ChagrinFalls.git
cd ChagrinFalls
```

### 2 — Build & test the backend

```bash
dotnet build
dotnet test src/ChagrinFalls.Tests
```

All 37 unit tests should pass. The backend has no external dependencies beyond the .NET SDK.

### 3 — Open the Godot project

1. Launch **Godot 4.6.1** (the .NET build).
2. Click **Import** and navigate to `godot/ChagrinFalls.Godot/project.godot`.
3. Godot will prompt you to build the C# solution — click **Build** (or press **Alt+B**).
4. Press **F5** (or the ▶ button) to run the game.

You should see a dark screen with a dialogue panel at the bottom. The opening conversation with Detective Moore will start immediately, including a conditional choice that appears only when the player has a specific item in their inventory.

### 4 — Explore the dialogue system

The dialogue system lives entirely in the backend library and can be used independently of Godot:

```csharp
var state   = new GameState();
var manager = new DialogueManager(state);

manager.OnDialogueLineChanged += line =>
    Console.WriteLine($"{line.Speaker}: {line.Text}");
manager.OnConversationEnded += () =>
    Console.WriteLine("Conversation ended.");

manager.StartEvent(myConversationEvent);

// Advance through lines without choices:
manager.Advance();

// Or pick a choice when choices are present:
foreach (var choice in manager.AvailableChoices)
    Console.WriteLine($"  [{manager.AvailableChoices.IndexOf(choice)}] {choice.Text}");
manager.SelectChoice(0);
```

#### Built-in condition types

| `ConditionType` | Required parameter | Description |
|-----------------|--------------------|-------------|
| `ItemInInventory` | `itemId` | True when the player carries the named item |
| `InformationLearned` | `informationId` | True when the player has recorded the named entry in their journal |

You can register additional condition types by implementing `IConditionEvaluator` and calling `manager.RegisterEvaluator(myEvaluator)`.