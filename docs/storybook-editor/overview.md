﻿# Storybook Editor — Overview

The **Chagrin Falls Storybook Editor** is a standalone Godot application (`godot/ChagrinFalls.StoryEditor`) for designing and saving storybooks without writing JSON by hand.

---

## Launching the editor

Open `godot/ChagrinFalls.StoryEditor` as a Godot project and press **Run**. The editor starts maximised.

---

## Window layout

The editor is divided into three areas:

| Area | Position | Purpose |
|---|---|---|
| **Toolbar** | Top bar | 📄 New · 📂 Open · 💾 Save · 💾 Save As… · current file path |
| **Structure tree** | Left panel | Navigate all elements of the storybook |
| **Property form** | Right panel | Edit the selected element; content changes based on selection |

| Area | Purpose |
|---|---|
| **Toolbar** | Create, open, save, or save-as a storybook JSON file |
| **Structure tree** (left) | Navigate between all elements of the storybook |
| **Property form** (right) | Edit the selected element |

---

## Structure tree branches

| Icon | Branch | What it contains |
|---|---|---|
| 📖 | Storybook root | Click to edit title, description, ID, starting location |
| 📍 | Locations | One entry per location; click to edit or add POIs |
| 💬 | Conversation Events | One entry per event; click to edit dialogue |
| 🧑 | Characters | Storybook-level character definitions |
| 📦 | Items | Storybook-level item definitions |
| 📔 | Journal Entries | Storybook-level journal entry definitions |

Click the **＋ Add …** entry at the bottom of each branch to create a new element.

---

## Saving

Press **💾 Save** (or **Ctrl+S** is not yet bound — use the button). The file is written as indented camelCase JSON to the path shown in the toolbar. The storybook `id` is automatically set from the filename.

To save to a new location use **💾 Save As…**.

The game reads storybooks from `godot/ChagrinFalls.Godot/Storybooks/`. Save directly there to test immediately.

---

## Next steps

- [Characters & Items](characters-and-items.md)
- [Locations & Points of Interest](locations.md)
- [Conversation Events](conversations.md)
- [Dialogue Lines, Choices & Conditions](dialogue.md)
- [Effects & Conditional Branching](effects-and-branches.md)

