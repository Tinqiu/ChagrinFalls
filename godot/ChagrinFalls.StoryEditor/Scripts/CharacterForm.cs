using ChagrinFalls.Backend.Models;
using Godot;
namespace ChagrinFalls.StoryEditor.Scripts;
/// <summary>Right-panel form for editing a storybook-level <see cref="Character"/>.</summary>
public partial class CharacterForm : VBoxContainer
{
    private EditorState _state       = null!;
    private string      _characterId = string.Empty;
    private bool        _loading;
    private LineEdit _nameEdit        = null!;
    private LineEdit _descriptionEdit = null!;
    private LineEdit _idEdit          = null!;
    public void Initialise(EditorState state, string characterId)
    {
        _state       = state;
        _characterId = characterId;
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
        if (!_state.Storybook.Characters.TryGetValue(_characterId, out var c)) return;
        _loading = true;
        _nameEdit.Text        = c.Name;        _nameEdit.CaretColumn        = _nameEdit.Text.Length;
        _descriptionEdit.Text = c.Description; _descriptionEdit.CaretColumn = _descriptionEdit.Text.Length;
        _idEdit.Text          = c.Id;
        _loading = false;
    }
    private void BuildUi()
    {
        var header = new Label { Text = "Character" };
        header.AddThemeFontSizeOverride("font_size", 20);
        AddChild(header);
        AddChild(new HSeparator());
        _nameEdit        = AddField("Name");
        _descriptionEdit = AddField("Description");
        _idEdit          = AddField("ID (auto)");
        _idEdit.Editable = false;
        var delBtn = new Button { Text = "🗑 Delete character" };
        delBtn.Pressed += () => _state.RemoveCharacter(_characterId);
        AddChild(delBtn);
        _nameEdit.TextSubmitted        += v => CommitName(v);
        _nameEdit.FocusExited          += () => CommitName(_nameEdit.Text);
        _descriptionEdit.TextSubmitted += v => CommitDescription(v);
        _descriptionEdit.FocusExited   += () => CommitDescription(_descriptionEdit.Text);
    }

    private void CommitName(string v)
    {
        if (_loading || string.IsNullOrWhiteSpace(v)) return;
        _characterId = _state.RenameCharacter(_characterId, v);
    }

    private void CommitDescription(string v)
    {
        if (_loading || !_state.Storybook.Characters.TryGetValue(_characterId, out var c)) return;
        c.Description = v;
        _state.UpdateCharacter(c);
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
