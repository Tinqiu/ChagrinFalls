using ChagrinFalls.Backend.Systems;
using Godot;
namespace ChagrinFalls.StoryEditor.Scripts;
/// <summary>
/// Root scene script. Owns the toolbar (New/Open/Save/SaveAs), the left-panel
/// <see cref="StructureTree"/>, and the right-panel property form area.
/// Swaps the active form when the tree selection changes.
/// </summary>
public partial class StoryEditorMain : Control
{
    private EditorState             _state           = null!;
    private StructureTree           _tree            = null!;
    private ScrollContainer         _rightScroll     = null!;
    private Label                   _filePathLabel   = null!;
    private FileDialog              _openDialog      = null!;
    private FileDialog              _saveDialog      = null!;
    private string _currentFilePath = string.Empty;
    public override void _Ready()
    {
        _state = new EditorState();
        AddChild(_state);
        BuildLayout();
        _state.Changed += OnStateChanged;
        _tree.Initialise(_state);
        _tree.SelectionChanged += OnTreeSelectionChanged;
        ShowMetaForm();
    }
    // ── Layout construction ───────────────────────────────────────────────────
    private void BuildLayout()
    {
        AnchorRight  = 1; AnchorBottom = 1;
        GrowHorizontal = GrowDirection.Both;
        GrowVertical   = GrowDirection.Both;
        LayoutDirection = LayoutDirectionEnum.Ltr;
        var root = new VBoxContainer
            { AnchorRight = 1, AnchorBottom = 1,
              GrowHorizontal = GrowDirection.Both, GrowVertical = GrowDirection.Both };
        AddChild(root);
        // ── Toolbar ───────────────────────────────────────────────────────────
        var toolbar = new HBoxContainer();
        toolbar.AddThemeConstantOverride("separation", 8);
        root.AddChild(toolbar);
        var newBtn    = new Button { Text = "📄 New" };
        var openBtn   = new Button { Text = "📂 Open" };
        var saveBtn   = new Button { Text = "💾 Save" };
        var saveAsBtn = new Button { Text = "💾 Save As…" };
        _filePathLabel = new Label { Text = "(unsaved)", SizeFlagsHorizontal = SizeFlags.ExpandFill };
        toolbar.AddChild(newBtn);
        toolbar.AddChild(openBtn);
        toolbar.AddChild(saveBtn);
        toolbar.AddChild(saveAsBtn);
        toolbar.AddChild(_filePathLabel);
        newBtn.Pressed    += OnNew;
        openBtn.Pressed   += OnOpen;
        saveBtn.Pressed   += OnSave;
        saveAsBtn.Pressed += OnSaveAs;
        // ── Split ─────────────────────────────────────────────────────────────
        var split = new HSplitContainer
            { SizeFlagsVertical = SizeFlags.ExpandFill,
              SizeFlagsHorizontal = SizeFlags.ExpandFill };
        split.SplitOffsets = [280];
        root.AddChild(split);
        // Left panel
        var leftPanel = new VBoxContainer { CustomMinimumSize = new Vector2(240, 0) };
        _tree = new StructureTree
            { SizeFlagsVertical = SizeFlags.ExpandFill,
              SizeFlagsHorizontal = SizeFlags.ExpandFill,
              CustomMinimumSize = new Vector2(0, 400) };
        leftPanel.AddChild(_tree);
        split.AddChild(leftPanel);
        // Right panel
        _rightScroll = new ScrollContainer
            { SizeFlagsVertical = SizeFlags.ExpandFill,
              SizeFlagsHorizontal = SizeFlags.ExpandFill };
        split.AddChild(_rightScroll);
        // ── File dialogs ──────────────────────────────────────────────────────
        _openDialog = new FileDialog
            { FileMode = FileDialog.FileModeEnum.OpenFile, Access = FileDialog.AccessEnum.Filesystem };
        _openDialog.AddFilter("*.json", "Storybook JSON");
        _openDialog.FileSelected += OnFileOpened;
        AddChild(_openDialog);
        _saveDialog = new FileDialog
            { FileMode = FileDialog.FileModeEnum.SaveFile, Access = FileDialog.AccessEnum.Filesystem };
        _saveDialog.AddFilter("*.json", "Storybook JSON");
        _saveDialog.FileSelected += OnFileSaved;
        AddChild(_saveDialog);
    }
    // ── Right-panel form management ───────────────────────────────────────────

    private void ClearRightPanel()
    {
        if (_rightScroll.GetChildCount() > 0)
            _rightScroll.GetChild(0).QueueFree();
    }

    private MarginContainer MakeMargin(Control form)
    {
        var margin = new MarginContainer
        {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            LayoutDirection     = LayoutDirectionEnum.Ltr,
        };
        margin.AddThemeConstantOverride("margin_left",   24);
        margin.AddThemeConstantOverride("margin_right",  24);
        margin.AddThemeConstantOverride("margin_top",    16);
        margin.AddThemeConstantOverride("margin_bottom", 16);
        margin.AddChild(form);
        return margin;
    }

    private void ShowMetaForm()
    {
        ClearRightPanel();
        var form = new StorybookMetaForm { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _rightScroll.AddChild(MakeMargin(form));
        form.Initialise(_state);
    }

    private void ShowLocationForm(string locationId)
    {
        ClearRightPanel();
        var form = new LocationForm { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _rightScroll.AddChild(MakeMargin(form));
        form.Initialise(_state, locationId);
    }

    private void ShowConversationEventForm(string eventId)
    {
        ClearRightPanel();
        var form = new ConversationEventForm { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _rightScroll.AddChild(MakeMargin(form));
        form.Initialise(_state, eventId);
    }

    private void ShowCharacterForm(string characterId)
    {
        ClearRightPanel();
        var form = new CharacterForm { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _rightScroll.AddChild(MakeMargin(form));
        form.Initialise(_state, characterId);
    }

    private void ShowItemForm(string itemId)
    {
        ClearRightPanel();
        var form = new ItemForm { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        _rightScroll.AddChild(MakeMargin(form));
        form.Initialise(_state, itemId);
    }

    // ── Event handlers ────────────────────────────────────────────────────────
    private void OnStateChanged() => _tree.Refresh();
    private void OnTreeSelectionChanged(int kind, string id)
    {
        switch ((StructureTree.NodeKind)kind)
        {
            case StructureTree.NodeKind.StorybookMeta:     ShowMetaForm();                break;
            case StructureTree.NodeKind.Location:          ShowLocationForm(id);          break;
            case StructureTree.NodeKind.ConversationEvent: ShowConversationEventForm(id); break;
            case StructureTree.NodeKind.Character:         ShowCharacterForm(id);         break;
            case StructureTree.NodeKind.Item:              ShowItemForm(id);              break;
        }
    }
    private void OnNew()
    {
        _state.NewStory();
        _currentFilePath = string.Empty;
        _filePathLabel.Text = "(unsaved)";
        ShowMetaForm();
    }
    private void OnOpen()  => _openDialog.Popup();
    private void OnSaveAs() => _saveDialog.Popup();
    private void OnSave()
    {
        if (string.IsNullOrEmpty(_currentFilePath))
            _saveDialog.Popup();
        else
            SaveTo(_currentFilePath);
    }
    private void OnFileOpened(string path)
    {
        try
        {
            var loader   = new StorybookLoader();
            var storybook = loader.LoadFile(path);
            _state.LoadStory(storybook);
            _currentFilePath    = path;
            _filePathLabel.Text = path;
            ShowMetaForm();
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to open storybook: {ex.Message}");
        }
    }
    private void OnFileSaved(string path)
    {
        _currentFilePath    = path;
        _filePathLabel.Text = path;
        SaveTo(path);
    }
    private void SaveTo(string path)
    {
        try
        {
            // Sync the storybook ID from the file name.
            var id = System.IO.Path.GetFileNameWithoutExtension(path);
            _state.Storybook.Id = id;
            new StorybookSaver().Save(_state.Storybook, path);
            GD.Print($"Saved to {path}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to save storybook: {ex.Message}");
        }
    }
}
