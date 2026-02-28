# Chagrin Falls

A text-based mystery game built with **Godot 4** and a pure C# backend.

---

## Project layout

| Folder | Purpose |
|---|---|
| `src/ChagrinFalls.Backend` | Pure C# game logic — models, systems, conditions |
| `src/ChagrinFalls.Tests` | xUnit test suite |
| `godot/ChagrinFalls.Godot` | Godot 4 project — scenes and UI scripts |
| `docs/` | This documentation site |

## Quick start

```bash
# Build everything
dotnet build

# Run tests
dotnet test src/ChagrinFalls.Tests

# Serve docs locally
uv run mkdocs serve
```

## Key concepts

- **[Dialogue Manager](systems/dialogue-manager.md)** — drives conversation flow, condition evaluation, and player choices
- **[Location Manager](systems/location-manager.md)** — tracks the current location, travel between locations, and item pickup from points of interest
- **[Player Inventory](systems/player-inventory.md)** — tracks items the player carries
- **[Player Journal](systems/player-journal.md)** — records information the player has learned
- **[Conditions](systems/conditions.md)** — gate dialogue lines and choices behind runtime predicates

