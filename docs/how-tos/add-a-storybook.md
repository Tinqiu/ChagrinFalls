# How-To: Add a Storybook

A storybook is a self-contained JSON file that defines a complete storyline — its locations, points of interest, and conversation events. Drop a new `*.json` file into `godot/ChagrinFalls.Godot/Storybooks/` and it will appear automatically on the storybook selection screen the next time the game runs.

---

## 1. Create the JSON file

Create a new file in `godot/ChagrinFalls.Godot/Storybooks/`, e.g. `my-new-story.json`. All property names are **camelCase**.

```json
{
  "id": "my-new-story",
  "title": "My New Story",
  "description": "A one-line summary shown on the selection screen.",
  "startingLocationId": "loc_start",
  "locations": { },
  "conversationEvents": { }
}
```

`id` must be unique across all storybooks and must not be empty.

---

## 2. Add locations

Each location needs an `id`, a `name`, and a list of `pointsOfInterest`.

```json
"locations": {
  "loc_start": {
    "id": "loc_start",
    "name": "The Harbour",
    "pointsOfInterest": [
      {
        "id": "npc_captain",
        "name": "The Captain",
        "type": "character",
        "conversationEventId": "evt_captain"
      },
      {
        "id": "poi_manifest",
        "name": "Shipping Manifest",
        "type": "item",
        "itemId": "shipping_manifest"
      }
    ]
  }
}
```

`type` is either `"character"` or `"item"`:

| Type | Required field | Effect |
|---|---|---|
| `"character"` | `conversationEventId` | Opens the linked conversation when clicked |
| `"item"` | `itemId` | Removed from the location and added to inventory when clicked |

---

## 3. Add conversation events

Conversation events must be referenced by at least one character POI's `conversationEventId`. See [Add a Conversation](add-a-conversation.md) for a full guide to building dialogue lines and choices.

```json
"conversationEvents": {
  "evt_captain": {
    "id": "evt_captain",
    "conditions": [],
    "participants": [],
    "startingConversationId": "conv_captain_intro",
    "conversations": {
      "conv_captain_intro": {
        "id": "conv_captain_intro",
        "startingDialogueLineId": "line_greeting",
        "dialogueLines": {
          "line_greeting": {
            "id": "line_greeting",
            "speaker": "Captain",
            "text": "What brings you to my harbour?",
            "conditions": [],
            "choices": []
          }
        }
      }
    }
  }
}
```

---

## 4. Verify

Run the game — the new storybook title should appear on the selection screen. If it doesn't, check the Godot output panel for a `Failed to load storybook` error, which will include the file path and the reason (invalid JSON, missing `id`, etc.).

!!! tip
    The `StorybookLoader` skips files that fail to parse rather than crashing, so a malformed storybook won't prevent other storybooks from loading.

