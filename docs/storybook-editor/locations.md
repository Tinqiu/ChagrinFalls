# Locations & Points of Interest

Locations are the places a player can travel to. Each location contains **points of interest (POIs)** — characters to talk to or items to pick up.

---

## Creating a location

1. Expand **📍 Locations** in the structure tree.
2. Click **＋ Add location**.
3. The property form opens with a default name of *New Location*.

## Location properties

| Field | Description |
|---|---|
| **Name** | Display name shown to the player on the location and travel screens. |
| **ID** | Auto-generated slug from the name (e.g. `cop_station`). Updated automatically when you rename. |

### Renaming a location

Edit the **Name** field and press **Enter** or click away. The ID updates to match. If this location is set as the **Starting Location** in the storybook meta form, that reference is updated automatically.

---

## Setting the starting location

Click the **📖 Storybook** root in the structure tree. The **Starting Location** dropdown lists all defined locations — pick the one the player begins in.

---

## Points of interest

Each location has a **Points of Interest** section. A POI is either a **Character** (triggers a conversation) or an **Item** (added to inventory when interacted with).

### Adding a POI

Click **＋ Character** or **＋ Item** in the Points of Interest header row.

### Character POI

| Field | Description |
|---|---|
| **Character** | Dropdown of all storybook-level characters. Selecting one auto-sets the POI's display name. |
| **Conversation** | Dropdown of all conversation events. This is the conversation that starts when the player interacts with this POI. |

!!! warning "Missing conversation"
    A yellow **⚠ No conversation assigned** warning appears if a character POI has no conversation selected. The character will display a fallback message in-game rather than starting a dialogue. Assign a conversation event to resolve this.

### Item POI

| Field | Description |
|---|---|
| **Item** | Dropdown of all storybook-level items. Selecting one auto-sets the display name and the item ID that is added to inventory on pickup. |

### Deleting a POI

Click **🗑** on the right of the POI row.

---

## Tips

- A location with no POIs is valid — it can be a travel destination the player reaches but has nothing to interact with yet.
- The same character can appear as a POI in multiple locations with different conversation events each time.
- Item POIs are removed from the location when the player picks them up at runtime — they do not need to be removed from the editor.

