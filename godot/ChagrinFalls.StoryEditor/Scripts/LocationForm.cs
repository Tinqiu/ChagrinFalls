using ChagrinFalls.Backend.Models;
using Godot;

namespace ChagrinFalls.StoryEditor.Scripts;

/// <summary>
/// Right-panel form for editing a <see cref="Location"/> and its points of interest.
/// Each POI row shows name, type selector, and a context-sensitive linker:
///   Character → ConversationEvent dropdown
///   Item      → ItemId LineEdit
/// </summary>
public partial class LocationForm : VBoxContainer
{
    private EditorState _state     = null!;
    private string      _locationId = string.Empty;
    private bool        _loading;

    private LineEdit   _nameEdit  = null!;
    private VBoxContainer _poiList = null!;

    public void Initialise(EditorState state, string locationId)
    {
        _state      = state;
        _locationId = locationId;
        LayoutDirection = LayoutDirectionEnum.Ltr;
        _state.Changed += OnChanged;
        BuildUi();
        Refresh();
    }

    public override void _ExitTree() { if (_state != null) _state.Changed -= OnChanged; }

    private void OnChanged() => CallDeferred(MethodName.Refresh);

    public void Refresh()
    {
        if (!IsInsideTree()) return;
        if (!_state.Storybook.Locations.TryGetValue(_locationId, out var location)) return;
        _loading = true;
        _nameEdit.Text = location.Name;
        _nameEdit.CaretColumn = _nameEdit.Text.Length;
        RebuildPoiList(location);
        _loading = false;
    }

    private void BuildUi()
    {
        var header = new Label { Text = "Location" };
        header.AddThemeFontSizeOverride("font_size", 20);
        AddChild(header);
        AddChild(new HSeparator());

        var nameRow = new HBoxContainer();
        nameRow.AddChild(new Label { Text = "Name", CustomMinimumSize = new Vector2(120, 0) });
        _nameEdit = new LineEdit { SizeFlagsHorizontal = SizeFlags.ExpandFill }.Ltr();
        nameRow.AddChild(_nameEdit);
        AddChild(nameRow);

        _nameEdit.TextSubmitted += CommitName;
        _nameEdit.FocusExited   += ()   => CommitName(_nameEdit.Text);

        AddChild(new HSeparator());

        var poiHeader = new HBoxContainer();
        poiHeader.AddChild(new Label { Text = "Points of Interest" });
        poiHeader.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });

        var addCharBtn = new Button { Text = "＋ Character" };
        var addItemBtn = new Button { Text = "＋ Item" };
        addCharBtn.Pressed += () => _state.AddPointOfInterest(
            _locationId, "New Character", PointOfInterestType.Character);
        addItemBtn.Pressed += () => _state.AddPointOfInterest(
            _locationId, "New Item", PointOfInterestType.Item);
        poiHeader.AddChild(addCharBtn);
        poiHeader.AddChild(addItemBtn);
        AddChild(poiHeader);

        _poiList = new VBoxContainer();
        _poiList.AddThemeConstantOverride("separation", 4);
        AddChild(_poiList);
    }

    private void CommitName(string newName)
    {
        if (_loading || string.IsNullOrWhiteSpace(newName)) return;
        _locationId = _state.RenameLocation(_locationId, newName);
    }

    private void RebuildPoiList(Location location)
    {
        foreach (Node child in _poiList.GetChildren()) child.QueueFree();

        foreach (var poi in location.PointsOfInterest)
            _poiList.AddChild(BuildPoiRow(poi));
    }

    private Control BuildPoiRow(PointOfInterest poi)
    {
        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 2);

        var row = new HBoxContainer();

        // Type selector
        var typeBtn = new OptionButton { CustomMinimumSize = new Vector2(120, 0) };
        typeBtn.AddItem("Character");
        typeBtn.AddItem("Item");
        typeBtn.Selected = poi.Type == PointOfInterestType.Character ? 0 : 1;

        // Linker (swaps based on type)
        var linkerContainer = new HBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        BuildLinker(linkerContainer, poi, vbox);

        typeBtn.ItemSelected += idx =>
        {
            var updated  = Clone(poi);
            updated.Type = idx == 0 ? PointOfInterestType.Character : PointOfInterestType.Item;
            updated.CharacterId = null;
            updated.ConversationEventId = null;
            updated.ItemId = null;
            updated.Name   = string.Empty;
            _state.UpdatePointOfInterest(_locationId, updated);
        };
        row.AddChild(typeBtn);
        row.AddChild(linkerContainer);

        // Delete
        var delBtn = new Button { Text = "🗑" };
        delBtn.Pressed += () => _state.RemovePointOfInterest(_locationId, poi.Id);
        row.AddChild(delBtn);

        vbox.AddChild(row);
        return vbox;
    }

    private void BuildLinker(HBoxContainer container, PointOfInterest poi, VBoxContainer parent)
    {
        foreach (Node child in container.GetChildren()) child.QueueFree();

        if (poi.Type == PointOfInterestType.Character)
        {
            var charLbl = new Label { Text = "Character:" };
            var charDd  = new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill };
            DropdownHelper.PopulateCharacters(charDd, _state.Storybook, poi.CharacterId);

            var convLbl = new Label { Text = "  Conversation:" };
            var convDd  = new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill };
            DropdownHelper.PopulateConversationEvents(convDd, _state.Storybook, poi.ConversationEventId);

            // Warning label shown when no conversation is selected
            var warning = new Label
            {
                Text      = "⚠ No conversation assigned — character will not be interactive.",
                Visible   = string.IsNullOrEmpty(poi.ConversationEventId),
                Modulate  = new Color(1f, 0.75f, 0f),
            };
            parent.AddChild(warning);

            charDd.ItemSelected += _ =>
            {
                var updated = Clone(poi);
                updated.CharacterId = DropdownHelper.GetSelectedId(charDd);
                if (!string.IsNullOrEmpty(updated.CharacterId) &&
                    _state.Storybook.Characters.TryGetValue(updated.CharacterId, out var c))
                    updated.Name = c.Name;
                _state.UpdatePointOfInterest(_locationId, updated);
            };

            convDd.ItemSelected += _ =>
            {
                var updated = Clone(poi);
                updated.ConversationEventId = DropdownHelper.GetSelectedId(convDd);
                warning.Visible = string.IsNullOrEmpty(updated.ConversationEventId);
                _state.UpdatePointOfInterest(_locationId, updated);
            };

            container.AddChild(charLbl);
            container.AddChild(charDd);
            container.AddChild(convLbl);
            container.AddChild(convDd);
        }
        else
        {
            var itemLbl = new Label { Text = "Item:" };
            var itemDd  = new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill };
            DropdownHelper.PopulateItems(itemDd, _state.Storybook, poi.ItemId);
            itemDd.ItemSelected += _ =>
            {
                var updated = Clone(poi);
                updated.ItemId = DropdownHelper.GetSelectedId(itemDd);
                if (!string.IsNullOrEmpty(updated.ItemId) &&
                    _state.Storybook.Items.TryGetValue(updated.ItemId, out var it))
                    updated.Name = it.Name;
                _state.UpdatePointOfInterest(_locationId, updated);
            };
            container.AddChild(itemLbl);
            container.AddChild(itemDd);
        }
    }

    private static PointOfInterest Clone(PointOfInterest p) => new()
    {
        Id = p.Id, Name = p.Name, Type = p.Type,
        CharacterId = p.CharacterId,
        ConversationEventId = p.ConversationEventId, ItemId = p.ItemId,
    };
}

