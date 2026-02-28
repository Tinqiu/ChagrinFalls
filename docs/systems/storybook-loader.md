# Storybook Loader

`ChagrinFalls.Backend.Systems.StorybookLoader`

Discovers and deserialises [`Storybook`](#storybook-model) instances from a directory of JSON files. Each `*.json` file is expected to contain one complete storybook.

---

## Constructor

```csharp
public StorybookLoader()
```

No configuration required — serialisation options (case-insensitive property names, camelCase enum values) are set internally.

---

## Methods

### `LoadAll`

```csharp
public IReadOnlyList<Storybook> LoadAll(string directory, Action<string, Exception>? onError = null)
```

Scans `directory` for `*.json` files and deserialises each one. Files that fail to load are skipped rather than throwing; the optional `onError` callback receives the file path and exception for any failure.

Returns an empty list (not an exception) when the directory does not exist.

| Parameter | Description |
|---|---|
| `directory` | Absolute path to the directory to scan. |
| `onError` | Optional. Called with `(filePath, exception)` for each file that fails. |

Throws `ArgumentException` for null or whitespace `directory`.

### `LoadFile`

```csharp
public Storybook LoadFile(string filePath)
```

Loads and deserialises a single storybook from the given file path.

| Throws | When |
|---|---|
| `ArgumentException` | `filePath` is null or whitespace |
| `FileNotFoundException` | The file does not exist |
| `JsonException` | The file contains invalid JSON or deserialises to `null` |
| `InvalidOperationException` | The storybook `Id` is missing or whitespace |

---

## Storybook model

```csharp
public class Storybook
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string StartingLocationId { get; set; }
    public Dictionary<string, Location> Locations { get; set; }
    public Dictionary<string, ConversationEvent> ConversationEvents { get; set; }
}
```

| Property | Description |
|---|---|
| `Id` | Unique identifier (e.g. `"margery-and-the-spooky-shack"`). Must be non-empty. |
| `Title` | Display name shown on the storybook selection screen. |
| `Description` | Short summary shown as a tooltip on the selection screen. |
| `StartingLocationId` | Must match a key in `Locations`. |
| `Locations` | All locations keyed by their `id`. |
| `ConversationEvents` | All conversation events keyed by their `id`. |

---

## JSON format

Storybook files live in `godot/ChagrinFalls.Godot/Storybooks/`. Property names are **camelCase**; enum values (`type` on points of interest) are also **camelCase** (`"item"`, `"character"`).

```json
{
  "id": "my-story",
  "title": "My Story",
  "description": "A short description.",
  "startingLocationId": "loc_a",
  "locations": {
    "loc_a": {
      "id": "loc_a",
      "name": "The Old Mill",
      "pointsOfInterest": [
        {
          "id": "npc_miller",
          "name": "The Miller",
          "type": "character",
          "conversationEventId": "evt_miller"
        },
        {
          "id": "poi_gear",
          "name": "Broken Gear",
          "type": "item",
          "itemId": "broken_gear"
        }
      ]
    }
  },
  "conversationEvents": {
    "evt_miller": {
      "id": "evt_miller",
      "conditions": [],
      "participants": [],
      "startingConversationId": "conv_miller",
      "conversations": { }
    }
  }
}
```

See the [How-To: Add a Storybook](../how-tos/add-a-storybook.md) guide for a full walkthrough.

