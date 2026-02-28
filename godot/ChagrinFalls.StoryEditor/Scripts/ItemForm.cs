using ChagrinFalls.Backend.Models;
using Godot;
namespace ChagrinFalls.StoryEditor.Scripts;
/// <summary>Right-panel form for editing a storybook-level <see cref="Item"/>.</summary>
public partial class ItemForm : VBoxContainer
{
    private EditorState _state  = null!;
    private string      _itemId = string.Empty;
    private bool        _loading;
    private LineEdit _nameEdit        = null!;
    private LineEdit _descriptionEdit = null!;
    private LineEdit _idEdit          = null!;
    public void Initialise(EditorState state, string itemId)
    {
        _state  = state;
        _itemId = itemId;
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
        if (!_state.Storybook.Items.TryGetValue(_itemId, out var item)) return;
        _loading = true;
        _nameEdit.Text        = item.Name;        _nameEdit.CaretColumn        = _nameEdit.Text.Length;
        _descriptionEdit.Text = item.Description; _descriptionEdit.CaretColumn = _descriptionEdit.Text.Length;
        _idEdit.Text          = item.Id;
        _loading = false;
    }
    private void BuildUi()
    {
        var header = new Label { Text = "Item" };
        header.AddThemeFontSizeOverride("font_size", 20);
        AddChild(header);
        AddChild(new HSeparator());
        _nameEdit        = AddField("Name");
        _descriptionEdit = AddField("Description");
        _idEdit          = AddField("ID (auto)");
        _idEdit.Editable = false;
        var delBtn = new Button { Text = "🗑 Delete item" };
        delBtn.Pressed += () => _state.RemoveItem(_itemId);
        AddChild(delBtn);
        _nameEdit.TextSubmitted        += CommitName;
        _nameEdit.FocusExited          += () => CommitName(_nameEdit.Text);
        _descriptionEdit.TextSubmitted += CommitDescription;
        _descriptionEdit.FocusExited   += () => CommitDescription(_descriptionEdit.Text);
    }

    private void CommitName(string v)
    {
        if (_loading || string.IsNullOrWhiteSpace(v)) return;
        _itemId = _state.RenameItem(_itemId, v);
    }

    private void CommitDescription(string v)
    {
        if (_loading || !_state.Storybook.Items.TryGetValue(_itemId, out var item)) return;
        item.Description = v;
        _state.UpdateItem(item);
    }
    private LineEdit AddField(string label)
    {
        var row  = new HBoxContainer();
        var lbl  = new Label { Text = label, CustomMinimumSize = new Vector2(160, 0) };
        var edit = new LineEdit { SizeFlagsHorizontal = SizeFlags.ExpandFill }.Ltr();
        row.AddChild(lbl);
        row.AddChild(edit);
        AddChild(row);
        return edit;
    }
}
