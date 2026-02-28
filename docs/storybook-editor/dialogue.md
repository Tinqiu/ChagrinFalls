# Dialogue Lines, Choices & Conditions

Each **dialogue line** is one unit of speech. Lines are chained together to form the conversation flow. A line can optionally present **player choices**, and both lines and choices can be gated behind **conditions**.

---

## Adding a dialogue line

Inside a conversation section click **＋ Line**. A new panel appears at the bottom of the conversation.

## Dialogue line fields

| Field | Description |
|---|---|
| **Speaker** (dropdown) | Who says this line. Always includes **Player** plus every character defined in the storybook. |
| **Dialogue text** | The text the player sees. Press **Enter** or click away to save. |
| **Then go to** | The default next line when the player advances (no choices, no matching branches). Select **(end conversation)** to end here. |
| 🗑 | Delete this line. |

---

## Controlling flow — the three row-two buttons

Below the speaker/text row each line has a second row with:

| Button | What it adds |
|---|---|
| **＋ Branch** | A conditional override for "Then go to" — checked before the default next line |
| **＋ Effect** | Something that happens when the player advances *past* this line (e.g. add/remove an item) |
| **＋ Choice** | A player-selectable option that replaces the automatic advance |

See [Effects & Conditional Branching](effects-and-branches.md) for Branches and Effects.

---

## Choices

Choices appear when the player would otherwise auto-advance. As soon as one or more choices exist (and their conditions are met) the advance button is hidden and choice buttons are shown instead.

### Adding a choice

Click **＋ Choice** on the line that should present options.

### Choice fields

| Field | Description |
|---|---|
| **Choice text** | The text of the button shown to the player. Press **Enter** or click away to save. |
| **→** (dropdown) | The line to go to when this choice is selected. **(end conversation)** ends the conversation. |
| 🗑 | Delete this choice. |
| **＋ Condition** | Add a condition that must be met for this choice to appear. |

### Choice conditions

A choice is only shown to the player if **all** its conditions are met. To add a condition:

1. Click **＋ Condition** on the choice row.
2. A condition row appears with three dropdowns:

| Dropdown | Options | Description |
|---|---|---|
| **System** | Inventory / Journal | Which game system to query |
| **Operator** | contains / does not contain | Whether the player must have or not have the target |
| **Target** | (item dropdown) | The specific item (or journal entry) to check |

3. Click 🗑 to remove a condition.

Multiple conditions on the same choice are **AND**-ed — all must be true for the choice to appear.

---

## Line order and the "First line" dropdown

Lines are stored in the order they were created. The **First line** dropdown at the top of each conversation section controls which line plays first — it is independent of display order in the editor.

The **Then go to** dropdown on each line previews its target as `Speaker: first 40 characters of text` so you can read the flow without needing to know line IDs.

---

## Tips

- A line with no choices and **Then go to → (end conversation)** ends the conversation when the player clicks advance.
- A line with choices where **every** choice has **Then go to → (end conversation)** ends the conversation regardless of which option the player picks (useful for farewell lines).
- Deleting a line does not automatically fix up "Then go to" references in other lines — check the surrounding lines after deleting.

