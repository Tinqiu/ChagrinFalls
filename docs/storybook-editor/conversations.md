# Conversation Events

A **conversation event** is the top-level container for a piece of dialogue. It groups one or more **conversations**, each of which is an ordered chain of dialogue lines.

---

## Creating a conversation event

1. Expand **💬 Conversation Events** in the structure tree.
2. Click **＋ Add conversation event**.
3. The property form opens. Give it a meaningful **Name / ID** (e.g. `john_donut_intro`).

## Conversation event properties

| Field | Description |
|---|---|
| **Name / ID** | The unique identifier for this event. Auto-slugified (e.g. `John Donut Intro` → `john_donut_intro`). Also used as the label in the structure tree. |

Press **Enter** or click away to commit the name. All POI conversation references are updated automatically.

---

## Conversations within an event

An event can contain multiple conversations (e.g. an introduction conversation and a revisit conversation). The **Starting Conversation** (set automatically to the first one) is the one that plays when the event is triggered.

### Adding a conversation

Click **＋ Add Conversation** at the bottom of the event form.

### Conversation properties

| Field | Description |
|---|---|
| **Conversation** (title field) | The name/ID of this conversation within the event. Rename it just like any other field. |
| **First line** | Dropdown — select the dialogue line where this conversation begins. |

### Renaming a conversation

Edit the title field in the conversation header and press **Enter** or click away.

---

## Structure overview

```
Conversation Event  (e.g. john_donut_intro)
└── Conversation    (e.g. intro)
    ├── Dialogue Line  (speaker + text + next line + branches + effects + choices)
    ├── Dialogue Line
    └── ...
```

For editing dialogue lines, choices, conditions, branches and effects see:

- [Dialogue Lines, Choices & Conditions](dialogue.md)
- [Effects & Conditional Branching](effects-and-branches.md)

---

## Tips

- Use **multiple conversations within one event** when the same character has distinctly different things to say at different points in the story (e.g. before and after a key item is found), and you want to control which conversation plays via game logic rather than branching dialogue.
- For branching *within* a single run of dialogue (based on what the player has in their inventory) use **branches** on individual lines instead — see [Effects & Conditional Branching](effects-and-branches.md).

