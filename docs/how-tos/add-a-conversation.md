# How-To: Add a Conversation

This guide walks through adding a new `ConversationEvent` from scratch — defining the lines, wiring up conditions, and starting it from the Godot layer.

---

## 1. Define the dialogue lines

Build your lines bottom-up (end first) so you can reference IDs before assignment:

```csharp
var lineEnd = new DialogueLine
{
    Id      = "line_end",
    Speaker = "Constable Reeves",
    Text    = "Right then, on your way.",
};

var lineA = new DialogueLine
{
    Id                 = "line_a",
    Speaker            = "Constable Reeves",
    Text               = "Nothing to see here. Move along.",
    NextDialogueLineId = "line_end",
};

var lineB = new DialogueLine
{
    Id                 = "line_b",
    Speaker            = "Constable Reeves",
    Text               = "That badge! You're with the detective agency?",
    NextDialogueLineId = "line_end",
};
```

---

## 2. Add choices and conditions (optional)

Attach choices to any line that should branch. Gate choices or lines behind conditions as needed:

```csharp
var lineGreeting = new DialogueLine
{
    Id      = "line_greeting",
    Speaker = "Constable Reeves",
    Text    = "Halt! State your business.",
    Choices = new List<Choice>
    {
        new() {
            Id                 = "choice_tourist",
            Text               = "Just passing through.",
            NextDialogueLineId = "line_a",
        },
        new() {
            Id                 = "choice_badge",
            Text               = "I have a badge.",
            NextDialogueLineId = "line_b",
            Conditions = new List<Condition>
            {
                new() {
                    ConditionType = "ItemInInventory",
                    Parameters    = new Dictionary<string, string> { ["itemId"] = "detective_badge" }
                }
            }
        },
    }
};
```

---

## 3. Assemble the `Conversation`

```csharp
var conversation = new Conversation
{
    Id                     = "conv_reeves",
    StartingDialogueLineId = "line_greeting",
    DialogueLines = new Dictionary<string, DialogueLine>
    {
        [lineGreeting.Id] = lineGreeting,
        [lineA.Id]        = lineA,
        [lineB.Id]        = lineB,
        [lineEnd.Id]      = lineEnd,
    }
};
```

---

## 4. Wrap it in a `ConversationEvent`

```csharp
var evt = new ConversationEvent
{
    Id                     = "event_reeves",
    StartingConversationId = "conv_reeves",
    Conversations = new Dictionary<string, Conversation>
    {
        [conversation.Id] = conversation
    },
    // Optional: only available once the player has learned who Reeves is.
    Conditions = new List<Condition>
    {
        new() {
            ConditionType = "InformationLearned",
            Parameters    = new Dictionary<string, string> { ["informationId"] = "met_reeves" }
        }
    }
};
```

---

## 5. Start the event from Godot

Pass the event to `DialogueUI` via a `DialogueManager`:

```csharp
var manager = new DialogueManager(_gameState);
_dialogueUI.StartConversation(manager, evt);
```

`DialogueUI` will handle all rendering and input from this point. Subscribe to `DialogueUI.ConversationFinished` to react when the conversation ends.

---

## Tips

- All line and choice IDs must be unique **within their `Conversation`**.
- A `null` `NextDialogueLineId` (or a missing key) ends the conversation — use this for terminal lines instead of creating a dedicated end line.
- Conditions on a `DialogueLine` cause the line to be **skipped** (not blocked) — the manager follows `NextDialogueLineId` until it finds a line whose conditions are met.

