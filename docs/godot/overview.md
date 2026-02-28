# Godot Layer Overview

The Godot project (`godot/ChagrinFalls.Godot`) is a thin UI shell over the pure C# backend. It is responsible for rendering dialogue, managing the menu bar, and hosting overlay screens.

---

## Scene inventory

| Scene | Root type | Script | Purpose |
|---|---|---|---|
| `Scenes/Main.tscn` | `Control` | `Scripts/Main.cs` | Root scene. Bootstraps `GameState`, wires up UI. |
| `Scenes/DialogueUI.tscn` | `CanvasLayer` | `Scripts/DialogueUI.cs` | Displays the active dialogue line and player choices. |
| `Scenes/MenuBar.tscn` | `CanvasLayer` | `Scripts/MenuBar.cs` | Persistent right-hand sidebar with Journal and Backpack buttons. |
| `Scenes/InventoryScreen.tscn` | `CanvasLayer` (layer 10) | `Scripts/InventoryScreen.cs` | Modal overlay showing the player's inventory. Blocks input to lower layers while open. |

---

## CanvasLayer ordering

| Layer | Node | Notes |
|---|---|---|
| 1 (default) | `DialogueUI`, `MenuBar` | Normal UI layer |
| 10 | `InventoryScreen` | Renders above all other UI; full-screen overlay blocks clicks |

---

## Backend integration

The Godot scripts reference `ChagrinFalls.Backend` directly via a `<ProjectReference>`. No serialisation or network boundary exists — backend objects are instantiated and held in memory by `Main.cs` for the lifetime of the scene.

```
Main.cs
 └── GameState
      ├── PlayerInventory  ──▶  InventoryScreen.Open(inventory)
      └── PlayerJournal

 └── DialogueManager  ──▶  DialogueUI.StartConversation(manager, event)
```

