﻿# Characters & Items

Characters and items are **storybook-level definitions** — they exist independently of any location or conversation. Once defined, they can be referenced anywhere: as a point of interest in a location, as a speaker in dialogue, or as the target of an inventory condition or effect.

---

## Characters

### Creating a character

1. In the structure tree, expand the **🧑 Characters** branch.
2. Click **＋ Add character** at the bottom.
3. A new entry called *New Character* appears and the property form opens on the right.

### Character properties

| Field | Description |
|---|---|
| **Name** | Display name shown to the player and used in dialogue speaker dropdowns. |
| **ID** | Auto-generated slug from the name (e.g. `john_donut`). Read-only — updated automatically when you rename. |
| **Description** | Internal notes for the writer. Not shown to the player. |

### Renaming a character

Edit the **Name** field and press **Enter** or click away. The ID updates automatically to match (e.g. renaming *John Donut* sets the ID to `john_donut`). All location POIs that reference this character are updated automatically.

### Deleting a character

Click **🗑 Delete character** at the bottom of the form. Any POIs that referenced this character will have their character link cleared — check affected locations afterwards.

---

## Items

Items work identically to characters but live in the **📦 Items** branch.

### Creating an item

1. Expand **📦 Items** in the structure tree.
2. Click **＋ Add item**.
3. Fill in **Name** and optionally **Description**.

### Item properties

| Field | Description |
|---|---|
| **Name** | Display name shown in the inventory screen. |
| **ID** | Auto-generated from the name (e.g. `torn_letter`). Used as the inventory key at runtime. |
| **Description** | Internal notes. |

### Renaming an item

Same as characters — edit the name, press Enter or click away. All POI item links and inventory condition/effect references are updated automatically.

### Deleting an item

Click **🗑 Delete item**. Inventory conditions and effects that referenced this item will have their link cleared.

---

## Journal Entries

Journal entries are **storybook-level definitions** that capture important information the player learns throughout the story (clues, facts, story beats). Once defined, they can be referenced in conversation effects to add them to the player's journal.

### Creating a journal entry

1. In the structure tree, expand the **📔 Journal Entries** branch.
2. Click **＋ Add journal entry** at the bottom.
3. A new entry called *New Journal Entry* appears and the property form opens on the right.

### Journal entry properties

| Field | Description |
|---|---|
| **Title** | Display title shown to the player when viewing their journal. |
| **ID** | Auto-generated slug from the title (e.g. `victim_identified`). Read-only — updated automatically when you rename. |
| **Description** | The content or details of the journal entry; shown to the player in the journal UI. |

### Renaming a journal entry

Edit the **Title** field and press **Enter** or click away. The ID updates automatically to match (e.g. renaming *Victim Identified* sets the ID to `victim_identified`). All conversation effects that reference this entry are updated automatically.

### Deleting a journal entry

Click **🗑 Delete journal entry** at the bottom of the form. Any conversation effects that referenced this entry will have their link cleared.

---

## Tips

- Define **all characters, items, and journal entries first** before building locations or conversations — the dropdowns in those forms are populated from these definitions.
- Journal entry **IDs** are used at runtime to check whether the player has learned specific information via the `InformationLearned` condition. If you rename an entry in the editor, the ID changes and all references are kept consistent automatically.
- Use journal entries to help players track plot-critical information and guide them through the story's mystery.

