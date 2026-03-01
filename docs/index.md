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

### Storybook Editor

The visual authoring tool for building storylines without writing JSON by hand.

- **[Overview](storybook-editor/overview.md)** — window layout and saving
- **[Characters & Items](storybook-editor/characters-and-items.md)** — define reusable characters and inventory items
- **[Locations & Points of Interest](storybook-editor/locations.md)** — places the player can visit and things to interact with
- **[Conversation Events](storybook-editor/conversations.md)** — top-level dialogue containers
- **[Dialogue Lines, Choices & Conditions](storybook-editor/dialogue.md)** — the line-by-line conversation builder
- **[Effects & Conditional Branching](storybook-editor/effects-and-branches.md)** — inventory effects and automatic story gating

### Backend systems

- **[Storybook Loader](systems/storybook-loader.md)** — loads self-contained storylines from JSON files on disk and presents them on the selection screen
- **[Dialogue Manager](systems/dialogue-manager.md)** — drives conversation flow, condition evaluation, and player choices
- **[Location Manager](systems/location-manager.md)** — tracks the current location, travel between locations, and item pickup from points of interest
- **[Player Inventory](systems/player-inventory.md)** — tracks items the player carries
- **[Player Journal](systems/player-journal.md)** — records information the player has learned
- **[Day Tracker](systems/day-tracker.md)** — tracks the current day (Day 1, Day 2, etc.)
- **[Game Clock](systems/game-clock.md)** — tracks the current time of day with an API to advance time
- **[Conditions](systems/conditions.md)** — gate dialogue lines and choices behind runtime predicates
- **[Time and Day Conditions](systems/time-and-day-conditions.md)** — special conditions for gating content by time of day or game day

