# Godot Layer Overview

The Godot project (`godot/ChagrinFalls.Godot`) is a thin UI shell over the pure C# backend. It is responsible for the storybook selection screen, rendering the current location, dialogue, the menu bar, and overlay screens.

---

## Scene inventory

| Scene | Root type | Script | Purpose |
|---|---|---|---|
| `Scenes/Main.tscn` | `Control` | `Scripts/Main.cs` | Root scene. Loads storybooks from disk, presents the selection screen, then bootstraps `GameState` and `LocationManager` when a storybook is chosen. |
| `Scenes/StorybookSelectScreen.tscn` | `CanvasLayer` (layer 20) | `Scripts/StorybookSelectScreen.cs` | Full-screen overlay shown at startup. Lists available storybooks; emits `StorybookSelected` on selection and hides itself. |
| `Scenes/LocationScreen.tscn` | `Control` | `Scripts/LocationScreen.cs` | Displays the current location name and its points of interest. Character POIs start conversations; item POIs are picked up directly. |
| `Scenes/DialogueUI.tscn` | `CanvasLayer` | `Scripts/DialogueUI.cs` | Displays the active dialogue line and player choices. |
| `Scenes/MenuBar.tscn` | `CanvasLayer` | `Scripts/MenuBar.cs` | Persistent right-hand sidebar with Journal, Backpack, and Travel buttons. |
| `Scenes/InventoryScreen.tscn` | `CanvasLayer` (layer 10) | `Scripts/InventoryScreen.cs` | Modal overlay showing the player's inventory. Blocks input to lower layers while open. |
| `Scenes/TravelScreen.tscn` | `CanvasLayer` (layer 10) | `Scripts/TravelScreen.cs` | Modal overlay listing all reachable locations. Calls `LocationManager.TravelTo` and closes itself on selection. |

---

## CanvasLayer ordering

| Layer | Node | Notes |
|---|---|---|
| 1 (default) | `DialogueUI`, `MenuBar` | Normal UI layer |
| 10 | `InventoryScreen`, `TravelScreen` | Render above all other UI; full-screen overlay blocks clicks |
| 20 | `StorybookSelectScreen` | Topmost layer; shown at startup before any game state exists |

---

## Storybook files

Storybooks live in `godot/ChagrinFalls.Godot/Storybooks/` as `*.json` files. They are loaded at runtime by `StorybookLoader` — dropping a new file into that directory is all that is needed to add it to the selection screen. See [Add a Storybook](../how-tos/add-a-storybook.md) for the full format.

---

## Backend integration

The Godot scripts reference `ChagrinFalls.Backend` directly via a `<ProjectReference>`. No serialisation or network boundary exists — backend objects are instantiated and held in memory by `Main.cs` for the lifetime of the scene.

```
Main.cs
 └── StorybookLoader  ──▶  loads *.json from res://Storybooks/
      └── StorybookSelectScreen.Populate(storybooks)
           └── StorybookSelected(storybookId)
                └── GameState  (created fresh per storybook)
                     ├── PlayerInventory  ──▶  InventoryScreen.Open(inventory)
                     └── PlayerJournal

                └── LocationManager  ──▶  LocationScreen.Initialise(manager)
                     └── OnLocationChanged  ──▶  LocationScreen refreshes POI buttons
                                                  TravelScreen.Open(manager)

                └── DialogueManager  ──▶  DialogueUI.StartConversation(manager, event)
                     └── (created per conversation, driven by LocationScreen.CharacterInteracted)
```
