using Godot;

namespace ChagrinFalls.StoryEditor.Scripts;

/// <summary>
/// Manages the left-panel <see cref="Tree"/> that shows the storybook's structure:
/// Locations (with POI children) and Conversation Events (with Conversation children).
/// Emits <see cref="SelectionChanged"/> when the user clicks a node.
/// </summary>
public partial class StructureTree : Tree
{
    public enum NodeKind { StorybookMeta, Location, ConversationEvent, Character, Item }

    [Signal]
    public delegate void SelectionChangedEventHandler(int kind, string id);

    private EditorState _state = null!;

    // Tree item references for the static branch roots.
    private TreeItem _locationsRoot = null!;
    private TreeItem _eventsRoot = null!;

    public void Initialise(EditorState state)
    {
        _state = state;
        _state.Changed += () => CallDeferred(MethodName.Refresh);
        HideRoot = true;
        // _Ready may have already fired before Initialise was called; refresh now
        // that state is available. IsInsideTree() guard in Refresh() keeps this safe.
        CallDeferred(MethodName.Refresh);
    }

    public override void _Ready()
    {
        ItemSelected += OnItemSelected;
        Refresh();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void Refresh()
    {
        if (_state is null || !IsInsideTree()) return;

        Clear();

        var root = CreateItem();

        // ── Storybook meta ────────────────────────────────────────────────────
        var meta = CreateItem(root);
        meta.SetText(0, $"📖 {_state.Storybook.Title}");
        meta.SetMetadata(0, (int)NodeKind.StorybookMeta + "|");

        // ── Locations ─────────────────────────────────────────────────────────
        _locationsRoot = CreateItem(root);
        _locationsRoot.SetText(0, "📍 Locations");
        _locationsRoot.SetSelectable(0, false);

        foreach (var loc in _state.Storybook.Locations.Values)
        {
            var locItem = CreateItem(_locationsRoot);
            locItem.SetText(0, $"  {loc.Name}");
            locItem.SetMetadata(0, $"{(int)NodeKind.Location}|{loc.Id}");

            foreach (var poi in loc.PointsOfInterest)
            {
                var poiItem = CreateItem(locItem);
                var icon = poi.Type == Backend.Models.PointOfInterestType.Character ? "🧑" : "📦";
                poiItem.SetText(0, $"    {icon} {poi.Name}");
                poiItem.SetMetadata(0, $"{(int)NodeKind.Location}|{loc.Id}");
                poiItem.SetSelectable(0, false);
            }
        }

        var addLoc = CreateItem(_locationsRoot);
        addLoc.SetText(0, "    ＋ Add location");
        addLoc.SetMetadata(0, "add_location|");

        // ── Conversation events ───────────────────────────────────────────────
        _eventsRoot = CreateItem(root);
        _eventsRoot.SetText(0, "💬 Conversation Events");
        _eventsRoot.SetSelectable(0, false);

        foreach (var evt in _state.Storybook.ConversationEvents.Values)
        {
            var evtItem = CreateItem(_eventsRoot);
            evtItem.SetText(0, $"  {evt.Id}");
            evtItem.SetMetadata(0, $"{(int)NodeKind.ConversationEvent}|{evt.Id}");
        }

        var addEvt = CreateItem(_eventsRoot);
        addEvt.SetText(0, "    ＋ Add conversation event");
        addEvt.SetMetadata(0, "add_event|");

        // ── Characters ────────────────────────────────────────────────────────
        var charsRoot = CreateItem(root);
        charsRoot.SetText(0, "🧑 Characters");
        charsRoot.SetSelectable(0, false);

        foreach (var c in _state.Storybook.Characters.Values)
        {
            var cItem = CreateItem(charsRoot);
            cItem.SetText(0, $"  {c.Name}");
            cItem.SetMetadata(0, $"{(int)NodeKind.Character}|{c.Id}");
        }

        var addChar = CreateItem(charsRoot);
        addChar.SetText(0, "    ＋ Add character");
        addChar.SetMetadata(0, "add_character|");

        // ── Items ─────────────────────────────────────────────────────────────
        var itemsRoot = CreateItem(root);
        itemsRoot.SetText(0, "📦 Items");
        itemsRoot.SetSelectable(0, false);

        foreach (var it in _state.Storybook.Items.Values)
        {
            var iItem = CreateItem(itemsRoot);
            iItem.SetText(0, $"  {it.Name}");
            iItem.SetMetadata(0, $"{(int)NodeKind.Item}|{it.Id}");
        }

        var addItem = CreateItem(itemsRoot);
        addItem.SetText(0, "    ＋ Add item");
        addItem.SetMetadata(0, "add_item|");
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void OnItemSelected()
    {
        var item = GetSelected();
        if (item is null) return;
        var meta = (string)item.GetMetadata(0);
        var parts = meta.Split('|', 2);
        switch (parts[0])
        {
            case "add_location":  CallDeferred(MethodName._AddLocation);          return;
            case "add_event":     CallDeferred(MethodName._AddConversationEvent); return;
            case "add_character": CallDeferred(MethodName._AddCharacter);         return;
            case "add_item":      CallDeferred(MethodName._AddItem);              return;
        }
        if (int.TryParse(parts[0], out var kind))
            EmitSignal(SignalName.SelectionChanged, kind, parts.Length > 1 ? parts[1] : "");
    }

    private void _AddLocation()          => _state.AddLocation("New Location");
    private void _AddConversationEvent() => _state.AddConversationEvent("new_event");
    private void _AddCharacter()         => _state.AddCharacter("New Character");
    private void _AddItem()              => _state.AddItem("New Item");
}

