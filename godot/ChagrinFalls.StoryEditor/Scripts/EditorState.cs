using ChagrinFalls.Backend.Models;
using Godot;

namespace ChagrinFalls.StoryEditor.Scripts;

/// <summary>
/// Holds the storybook currently being edited and exposes helpers for mutating
/// its collections. Emits <see cref="Changed"/> after every mutation so that
/// the tree and all open forms can refresh themselves.
/// </summary>
public partial class EditorState : Node
{
    [Signal]
    public delegate void ChangedEventHandler();

    public Storybook Storybook { get; private set; } = NewStorybook();

    private bool _changePending;

    /// <summary>
    /// Schedules a single <see cref="Changed"/> signal for the end of the current frame.
    /// Multiple calls within the same frame are coalesced into one emission so that
    /// nodes are never in a blocked state when Refresh() runs.
    /// </summary>
    private void NotifyChanged()
    {
        if (_changePending) return;
        _changePending = true;
        CallDeferred(MethodName._EmitChanged);
    }

    private void _EmitChanged()
    {
        _changePending = false;
        EmitSignal(SignalName.Changed);
    }

    // ── Storybook lifecycle ───────────────────────────────────────────────────

    public void NewStory()
    {
        Storybook = NewStorybook();
        NotifyChanged();
    }

    public void LoadStory(Storybook storybook)
    {
        Storybook = storybook;
        NotifyChanged();
    }

    // ── Metadata ──────────────────────────────────────────────────────────────

    public void SetId(string id)
    {
        Storybook.Id = id;
        NotifyChanged();
    }

    public void SetTitle(string title)
    {
        Storybook.Title = title;
        NotifyChanged();
    }

    public void SetDescription(string description)
    {
        Storybook.Description = description;
        NotifyChanged();
    }

    public void SetStartingLocationId(string locationId)
    {
        Storybook.StartingLocationId = locationId;
        NotifyChanged();
    }

    // ── Characters ────────────────────────────────────────────────────────────

    public Character AddCharacter(string name)
    {
        var character = new Character { Id = GenerateId(name, Storybook.Characters.Keys), Name = name };
        Storybook.Characters[character.Id] = character;
        NotifyChanged();
        return character;
    }

    public void UpdateCharacter(Character updated)
    {
        Storybook.Characters[updated.Id] = updated;
        NotifyChanged();
    }

    /// <summary>Renames a character: updates Name, re-keys the dictionary, fixes POI references. Returns the new ID.</summary>
    public string RenameCharacter(string oldId, string newName)
    {
        if (!Storybook.Characters.TryGetValue(oldId, out var c)) return oldId;
        var newId = GenerateId(newName, Storybook.Characters.Keys.Where(k => k != oldId));
        Storybook.Characters.Remove(oldId);
        c.Id   = newId;
        c.Name = newName;
        Storybook.Characters[newId] = c;
        foreach (var loc in Storybook.Locations.Values)
            foreach (var poi in loc.PointsOfInterest.Where(p => p.CharacterId == oldId))
                poi.CharacterId = newId;
        NotifyChanged();
        return newId;
    }

    public void RemoveCharacter(string characterId)
    {
        Storybook.Characters.Remove(characterId);
        // Clear references from POIs
        foreach (var loc in Storybook.Locations.Values)
            foreach (var poi in loc.PointsOfInterest.Where(p => p.CharacterId == characterId))
                poi.CharacterId = null;
        NotifyChanged();
    }

    // ── Items ─────────────────────────────────────────────────────────────────

    public Item AddItem(string name)
    {
        var item = new Item { Id = GenerateId(name, Storybook.Items.Keys), Name = name };
        Storybook.Items[item.Id] = item;
        NotifyChanged();
        return item;
    }

    public void UpdateItem(Item updated)
    {
        Storybook.Items[updated.Id] = updated;
        NotifyChanged();
    }

    /// <summary>Renames an item: updates Name, re-keys the dictionary, fixes POI references. Returns the new ID.</summary>
    public string RenameItem(string oldId, string newName)
    {
        if (!Storybook.Items.TryGetValue(oldId, out var item)) return oldId;
        var newId = GenerateId(newName, Storybook.Items.Keys.Where(k => k != oldId));
        Storybook.Items.Remove(oldId);
        item.Id   = newId;
        item.Name = newName;
        Storybook.Items[newId] = item;
        foreach (var loc in Storybook.Locations.Values)
            foreach (var poi in loc.PointsOfInterest.Where(p => p.ItemId == oldId))
                poi.ItemId = newId;
        NotifyChanged();
        return newId;
    }

    public void RemoveItem(string itemId)
    {
        Storybook.Items.Remove(itemId);
        // Clear references from POIs
        foreach (var loc in Storybook.Locations.Values)
            foreach (var poi in loc.PointsOfInterest.Where(p => p.ItemId == itemId))
                poi.ItemId = null;
        NotifyChanged();
    }

    // ── Locations ─────────────────────────────────────────────────────────────

    public Location AddLocation(string name)
    {
        var location = new Location { Id = GenerateId(name, Storybook.Locations.Keys), Name = name };
        Storybook.Locations[location.Id] = location;
        NotifyChanged();
        return location;
    }

    /// <summary>Renames a location: updates Name, re-keys the dictionary, fixes StartingLocationId. Returns the new ID.</summary>
    public string RenameLocation(string oldId, string newName)
    {
        if (!Storybook.Locations.TryGetValue(oldId, out var location)) return oldId;
        var newId = GenerateId(newName, Storybook.Locations.Keys.Where(k => k != oldId));
        Storybook.Locations.Remove(oldId);
        location.Id   = newId;
        location.Name = newName;
        Storybook.Locations[newId] = location;
        if (Storybook.StartingLocationId == oldId)
            Storybook.StartingLocationId = newId;
        NotifyChanged();
        return newId;
    }

    public void RemoveLocation(string locationId)
    {
        Storybook.Locations.Remove(locationId);
        if (Storybook.StartingLocationId == locationId)
            Storybook.StartingLocationId = string.Empty;
        NotifyChanged();
    }

    // ── Points of interest ────────────────────────────────────────────────────

    public PointOfInterest AddPointOfInterest(string locationId, string name, PointOfInterestType type)
    {
        if (!Storybook.Locations.TryGetValue(locationId, out var location))
            throw new ArgumentException($"Location '{locationId}' not found.", nameof(locationId));
        var existing = location.PointsOfInterest.Select(p => p.Id);
        var poi = new PointOfInterest { Id = GenerateId(name, existing), Name = name, Type = type };
        location.PointsOfInterest.Add(poi);
        NotifyChanged();
        return poi;
    }

    public void UpdatePointOfInterest(string locationId, PointOfInterest updated)
    {
        if (!Storybook.Locations.TryGetValue(locationId, out var location)) return;
        var idx = location.PointsOfInterest.FindIndex(p => p.Id == updated.Id);
        if (idx < 0) return;
        location.PointsOfInterest[idx] = updated;
        NotifyChanged();
    }

    public void RemovePointOfInterest(string locationId, string poiId)
    {
        if (!Storybook.Locations.TryGetValue(locationId, out var location)) return;
        location.PointsOfInterest.RemoveAll(p => p.Id == poiId);
        NotifyChanged();
    }

    // ── Conversation events ───────────────────────────────────────────────────

    public ConversationEvent AddConversationEvent(string name)
    {
        var evt = new ConversationEvent
        {
            Id = GenerateId(name, Storybook.ConversationEvents.Keys),
        };
        // Add a default empty conversation to get started.
        var conv = new Conversation { Id = GenerateId("intro", evt.Conversations.Keys), StartingDialogueLineId = string.Empty };
        evt.Conversations[conv.Id] = conv;
        evt.StartingConversationId = conv.Id;
        Storybook.ConversationEvents[evt.Id] = evt;
        NotifyChanged();
        return evt;
    }

    public void UpdateConversationEventName(string eventId, string newName)
    {
        // ConversationEvent has no Name field; we store display name as the first
        // conversation's id hint. For the tree label we derive from the key — re-key.
        if (!Storybook.ConversationEvents.TryGetValue(eventId, out var evt)) return;
        var newId = GenerateId(newName, Storybook.ConversationEvents.Keys.Where(k => k != eventId));
        Storybook.ConversationEvents.Remove(eventId);
        evt.Id = newId;
        Storybook.ConversationEvents[newId] = evt;
        // Update any POIs that referenced the old id.
        foreach (var loc in Storybook.Locations.Values)
            foreach (var poi in loc.PointsOfInterest.Where(p => p.ConversationEventId == eventId))
                poi.ConversationEventId = newId;
        NotifyChanged();
    }

    public void RemoveConversationEvent(string eventId)
    {
        Storybook.ConversationEvents.Remove(eventId);
        NotifyChanged();
    }

    // ── Conversations & dialogue lines ────────────────────────────────────────

    /// <summary>Renames a conversation within an event, re-keying it and fixing StartingConversationId. Returns the new ID.</summary>
    public string RenameConversation(string eventId, string oldConvId, string newName)
    {
        if (!Storybook.ConversationEvents.TryGetValue(eventId, out var evt)) return oldConvId;
        if (!evt.Conversations.TryGetValue(oldConvId, out var conv)) return oldConvId;
        var newId = GenerateId(newName, evt.Conversations.Keys.Where(k => k != oldConvId));
        evt.Conversations.Remove(oldConvId);
        conv.Id = newId;
        evt.Conversations[newId] = conv;
        if (evt.StartingConversationId == oldConvId)
            evt.StartingConversationId = newId;
        NotifyChanged();
        return newId;
    }

    public Conversation AddConversation(string eventId, string name)
    {
        if (!Storybook.ConversationEvents.TryGetValue(eventId, out var evt)) return null!;
        var conv = new Conversation { Id = GenerateId(name, evt.Conversations.Keys) };
        evt.Conversations[conv.Id] = conv;
        NotifyChanged();
        return conv;
    }

    public void UpdateConversation(string eventId, Conversation updated)
    {
        if (!Storybook.ConversationEvents.TryGetValue(eventId, out var evt)) return;
        evt.Conversations[updated.Id] = updated;
        NotifyChanged();
    }

    public DialogueLine AddDialogueLine(string eventId, string conversationId, string speaker, string text)
    {
        if (!Storybook.ConversationEvents.TryGetValue(eventId, out var evt)) return null!;
        if (!evt.Conversations.TryGetValue(conversationId, out var conv)) return null!;
        var existing = conv.DialogueLines.Keys;
        var line = new DialogueLine
        {
            Id      = GenerateId($"{speaker}_line", existing),
            Speaker = speaker,
            Text    = text,
        };
        conv.DialogueLines[line.Id] = line;
        if (string.IsNullOrEmpty(conv.StartingDialogueLineId))
            conv.StartingDialogueLineId = line.Id;
        NotifyChanged();
        return line;
    }

    public void UpdateDialogueLine(string eventId, string conversationId, DialogueLine updated)
    {
        if (!Storybook.ConversationEvents.TryGetValue(eventId, out var evt)) return;
        if (!evt.Conversations.TryGetValue(conversationId, out var conv)) return;
        conv.DialogueLines[updated.Id] = updated;
        NotifyChanged();
    }

    public void RemoveDialogueLine(string eventId, string conversationId, string lineId)
    {
        if (!Storybook.ConversationEvents.TryGetValue(eventId, out var evt)) return;
        if (!evt.Conversations.TryGetValue(conversationId, out var conv)) return;
        conv.DialogueLines.Remove(lineId);
        if (conv.StartingDialogueLineId == lineId)
            conv.StartingDialogueLineId = conv.DialogueLines.Keys.FirstOrDefault() ?? string.Empty;
        NotifyChanged();
    }

    // ── ID generation ─────────────────────────────────────────────────────────

    /// <summary>
    /// Slugifies <paramref name="name"/> and appends a numeric suffix if it collides
    /// with any existing key.
    /// </summary>
    public static string GenerateId(string name, IEnumerable<string> existingIds)
    {
        var slug = Slugify(name);
        var taken = new HashSet<string>(existingIds, StringComparer.OrdinalIgnoreCase);
        if (!taken.Contains(slug)) return slug;
        for (var i = 2; ; i++)
        {
            var candidate = $"{slug}_{i}";
            if (!taken.Contains(candidate)) return candidate;
        }
    }

    private static string Slugify(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "item";
        return string.Concat(
            name.ToLowerInvariant()
                .Select(c => char.IsLetterOrDigit(c) ? c : '_'))
            .Trim('_');
    }

    private static Storybook NewStorybook() => new()
    {
        Id    = "new-storybook",
        Title = "New Storybook",
    };
}

