using Godot;

namespace ChagrinFalls.StoryEditor.Scripts;

/// <summary>
/// Right-panel form for editing storybook-level metadata:
/// title, description, and starting location (dropdown).
/// </summary>
public partial class StorybookMetaForm : VBoxContainer
{
    private EditorState _state = null!;
    private bool _loading;

    private LineEdit _titleEdit       = null!;
    private LineEdit _idEdit          = null!;
    private LineEdit _descriptionEdit = null!;
    private OptionButton _startingLocationDropdown = null!;

    public void Initialise(EditorState state)
    {
        _state = state;
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
        _loading = true;
        _titleEdit.Text       = _state.Storybook.Title;
        _titleEdit.CaretColumn = _titleEdit.Text.Length;
        _idEdit.Text          = _state.Storybook.Id;
        _idEdit.CaretColumn   = _idEdit.Text.Length;
        _descriptionEdit.Text = _state.Storybook.Description;
        _descriptionEdit.CaretColumn = _descriptionEdit.Text.Length;
        DropdownHelper.PopulateLocations(
            _startingLocationDropdown, _state.Storybook, _state.Storybook.StartingLocationId);
        _loading = false;
    }

    private void BuildUi()
    {
        var header = new Label { Text = "Storybook" };
        header.AddThemeFontSizeOverride("font_size", 20);
        AddChild(header);
        AddChild(new HSeparator());

        (_titleEdit, _)       = AddField("Title");
        (_idEdit, _)          = AddField("ID (auto)");
        _idEdit.Editable      = false;
        (_descriptionEdit, _) = AddField("Description");

        var startRow = new HBoxContainer();
        startRow.AddChild(new Label { Text = "Starting Location",
            CustomMinimumSize = new Vector2(160, 0) });
        _startingLocationDropdown = new OptionButton
            { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        startRow.AddChild(_startingLocationDropdown);
        AddChild(startRow);

        _titleEdit.TextChanged       += v => { if (!_loading) _state.SetTitle(v); };
        _titleEdit.FocusExited       += () => { if (!_loading) _state.SetTitle(_titleEdit.Text); };
        _descriptionEdit.TextChanged += v => { if (!_loading) _state.SetDescription(v); };
        _descriptionEdit.FocusExited += () => { if (!_loading) _state.SetDescription(_descriptionEdit.Text); };
        _startingLocationDropdown.ItemSelected += _ =>
        {
            if (!_loading)
                _state.SetStartingLocationId(
                    DropdownHelper.GetSelectedId(_startingLocationDropdown));
        };
    }

    private (LineEdit edit, HBoxContainer row) AddField(string label)
    {
        var row  = new HBoxContainer();
        var lbl  = new Label { Text = label, CustomMinimumSize = new Vector2(160, 0) };
        var edit = new LineEdit
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
        }.Ltr();
        row.AddChild(lbl);
        row.AddChild(edit);
        AddChild(row);
        return (edit, row);
    }
}
