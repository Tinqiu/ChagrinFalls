﻿# Effects & Conditional Branching

Beyond player choices, the editor supports two more tools for making dialogue reactive to game state:

| Tool | When it fires | What it does |
|---|---|---|
| **Effect** | When the player advances *past* a line | Mutates game state (e.g. adds or removes an item) |
| **Branch** | When the player advances and no choices are shown | Overrides the default "Then go to" target based on conditions |

Both are added via the second row of a dialogue line (the row that contains **Then go to**).

---

## Effects

An effect fires the moment the player advances past the line — whether by clicking the advance button or by selecting a choice. Effects apply **before** navigation to the next line.

### Adding an effect

Click **＋ Effect** on the line. An effect row appears marked with ⚡.

### Effect row fields

| Dropdown | Options | Description |
|---|---|---|
| **System** | Inventory / Time | Which game system the effect targets |
| **Action** | Add item / Remove item / Advance time | Whether to add/remove an item or advance the game clock |
| **Target/Value** | Varies by action | For items: item dropdown; for time: hours and minutes input fields |

Click 🗑 to remove an effect.

### Multiple effects

A line can have multiple effects — they are applied in order from top to bottom.

### Example — Inventory Effect

> *"Here you go!"* — line has an effect **Inventory → Remove item → Donut**  
> When the player advances past this line the donut is removed from the inventory before the next line displays.

### Example — Time Effect

> *"Let me think about that..."* — line has an effect **Time → Advance time → 1 hour, 30 minutes**  
> When the player advances past this line, 1 hour and 30 minutes are added to the game clock before the next line displays.

---

## Conditional Branches

A branch lets you send the conversation down a different path depending on game state, **without** requiring player input. Branches are evaluated **in order** when the player advances; the first branch whose conditions all match is taken. If no branch matches, the line's **Then go to** fallback is used.

### Adding a branch

Click **＋ Branch** on the line. A branch row appears marked with ⤷.

### Branch row fields

| Field | Description |
|---|---|
| **If conditions met, go to** | Dropdown of all lines in the conversation (same previews as "Then go to"). Select **(end conversation)** to end if conditions match. |
| **＋ Condition** | Add a condition to this branch. |
| 🗑 | Delete this branch. |

### Branch conditions

Each branch condition has the same three-dropdown structure as choice conditions:

| Dropdown | Options | Description |
|---|---|---|
| **System** | Inventory / Journal | Which system to query |
| **Operator** | contains / does not contain | Presence check |
| **Target** | (item dropdown) | The specific item or journal entry |

Multiple conditions on the same branch are **AND**-ed.

### Evaluation order

Branches are evaluated top-to-bottom. The **first** branch whose conditions all pass is taken. Reorder branches by deleting and re-adding (reorder UI is not yet implemented).

### Example — inventory-gated path

```
Line: "What do you want?"
  ⤷ Branch: [Inventory does not contain Donut] → go to "I'm hungry, go away."
  Then go to: "Hi, I'm Detective Moore."
```

When the player does **not** have a donut the branch fires and the grumpy line plays.  
When the player **does** have a donut no branch matches and the conversation continues normally.

---

## Branches vs Choices

| | Branch | Choice |
|---|---|---|
| **Who decides?** | Game (automatic) | Player (button click) |
| **When evaluated?** | On advance, before default next line | Only shown when conditions met |
| **Visible to player?** | No | Yes (as a button) |

Use **branches** for automatic story gating (the player has/doesn't have something).  
Use **choices** for player agency (the player picks what to say or do).

---

## Tips

- An unconditional branch (no conditions added) **always** fires. Use this to unconditionally redirect a line to a target other than "Then go to" — useful as a final catch-all at the end of a branch list.
- Effects and branches interact in order: effects fire first, then the branch evaluation reads the **updated** game state. This means you can add an item on a line and then immediately branch based on having it.

