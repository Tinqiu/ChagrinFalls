using ChagrinFalls.Backend.Models;
using Godot;
namespace ChagrinFalls.StoryEditor.Scripts;
/// <summary>
/// Right-panel form for editing a <see cref="ConversationEvent"/>.
/// </summary>
public partial class ConversationEventForm : VBoxContainer
{
    private EditorState _state   = null!;
    private string      _eventId = string.Empty;
    public void Initialise(EditorState state, string eventId)
    {
        _state   = state;
        _eventId = eventId;
        LayoutDirection = LayoutDirectionEnum.Ltr;
        _state.Changed += OnChanged;
        Refresh();
    }

    public override void _ExitTree() => _state.Changed -= OnChanged;

    private void OnChanged() => CallDeferred(MethodName.Refresh);
    public void Refresh()
    {
        if (!IsInsideTree()) return;
        foreach (Node child in GetChildren()) child.QueueFree();
        if (!_state.Storybook.ConversationEvents.TryGetValue(_eventId, out var evt)) return;
        // ── Event-level header + rename ───────────────────────────────────────
        var header = new Label { Text = "Conversation Event" };
        header.AddThemeFontSizeOverride("font_size", 20);
        AddChild(header);
        var nameRow = new HBoxContainer();
        nameRow.AddChild(new Label { Text = "Name / ID:", CustomMinimumSize = new Vector2(100, 0) });
        var nameEdit = new LineEdit { Text = _eventId, SizeFlagsHorizontal = SizeFlags.ExpandFill }.Ltr();
        nameEdit.CaretColumn = nameEdit.Text.Length;
        nameEdit.TextSubmitted += v => CommitEventName(v);
        nameEdit.FocusExited   += () => CommitEventName(nameEdit.Text);
        nameRow.AddChild(nameEdit);
        AddChild(nameRow);
        AddChild(new HSeparator());
        foreach (var conv in evt.Conversations.Values)
            AddChild(BuildConversationSection(evt, conv));
        var addConvBtn = new Button { Text = "＋ Add Conversation" };
        addConvBtn.Pressed += () => _state.AddConversation(_eventId, "new_conversation");
        AddChild(addConvBtn);
    }
    private void CommitEventName(string v)
    {
        if (string.IsNullOrWhiteSpace(v) || v == _eventId) return;
        // Compute what the new ID will be before the rename so we can track it.
        var newId = EditorState.GenerateId(v, _state.Storybook.ConversationEvents.Keys.Where(k => k != _eventId));
        _state.UpdateConversationEventName(_eventId, v);
        _eventId = newId;
    }
    // ── Conversation section ──────────────────────────────────────────────────
    private Control BuildConversationSection(ConversationEvent evt, Conversation conv)
    {
        // We use a mutable local so rename can update the id inside closures.
        var convId = conv.Id;
        var section = new VBoxContainer();
        section.AddThemeConstantOverride("separation", 4);
        // Header row: title field + "First line:" dropdown + add-line button
        var convHeader = new HBoxContainer();
        var titleLbl = new Label { Text = "Conversation:", CustomMinimumSize = new Vector2(110, 0) };
        convHeader.AddChild(titleLbl);
        var titleEdit = new LineEdit
            { Text = conv.Id, CustomMinimumSize = new Vector2(160, 0) }.Ltr();
        titleEdit.CaretColumn = titleEdit.Text.Length;
        titleEdit.TextSubmitted += v => { if (!string.IsNullOrWhiteSpace(v)) convId = _state.RenameConversation(_eventId, convId, v); };
        titleEdit.FocusExited   += () => { var v = titleEdit.Text; if (!string.IsNullOrWhiteSpace(v)) convId = _state.RenameConversation(_eventId, convId, v); };
        convHeader.AddChild(titleEdit);
        convHeader.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        convHeader.AddChild(new Label { Text = "First line:" });
        var startDd = new OptionButton { CustomMinimumSize = new Vector2(220, 0) };
        PopulateLineDropdownWithLabels(startDd, conv, conv.StartingDialogueLineId);
        startDd.ItemSelected += _ =>
        {
            conv.StartingDialogueLineId = DropdownHelper.GetSelectedId(startDd);
            _state.UpdateConversation(_eventId, conv);
        };
        convHeader.AddChild(startDd);
        var addLineBtn = new Button { Text = "＋ Line" };
        addLineBtn.Pressed += () => _state.AddDialogueLine(_eventId, convId, "NPC", "");
        convHeader.AddChild(addLineBtn);
        section.AddChild(convHeader);
        section.AddChild(new HSeparator());
        foreach (var line in conv.DialogueLines.Values)
            section.AddChild(BuildLineRow(convId, conv, line));
        return section;
    }
    // ── Dialogue line row ─────────────────────────────────────────────────────
    private Control BuildLineRow(string convId, Conversation conv, DialogueLine line)
    {
        var panel = new PanelContainer();
        var vbox  = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 4);
        panel.AddChild(vbox);
        // Row 1: speaker dropdown + text + delete
        var row1 = new HBoxContainer();
        var speakerDd = new OptionButton { CustomMinimumSize = new Vector2(150, 0) };
        PopulateSpeakers(speakerDd, line.Speaker);
        speakerDd.ItemSelected += _ =>
        {
            line.Speaker = (string)speakerDd.GetItemMetadata(speakerDd.Selected);
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        var textEdit = new LineEdit
            { Text = line.Text, SizeFlagsHorizontal = SizeFlags.ExpandFill,
              PlaceholderText = "Dialogue text" }.Ltr();
        textEdit.CaretColumn = textEdit.Text.Length;
        textEdit.TextSubmitted += v => { line.Text = v; _state.UpdateDialogueLine(_eventId, convId, line); };
        textEdit.FocusExited   += () => { line.Text = textEdit.Text; _state.UpdateDialogueLine(_eventId, convId, line); };
        var delBtn = new Button { Text = "🗑" };
        delBtn.Pressed += () => _state.RemoveDialogueLine(_eventId, convId, line.Id);
        row1.AddChild(speakerDd);
        row1.AddChild(textEdit);
        row1.AddChild(delBtn);
        vbox.AddChild(row1);
        // Row 2: "Then go to:" dropdown + add-choice button
        var row2 = new HBoxContainer();
        row2.AddChild(new Label { Text = "Then go to:", CustomMinimumSize = new Vector2(90, 0) });
        var nextDd = new OptionButton { CustomMinimumSize = new Vector2(240, 0) };
        PopulateLineDropdownWithLabels(nextDd, conv, line.NextDialogueLineId, excludeId: line.Id);
        nextDd.ItemSelected += _ =>
        {
            line.NextDialogueLineId = DropdownHelper.GetSelectedId(nextDd);
            if (string.IsNullOrEmpty(line.NextDialogueLineId)) line.NextDialogueLineId = null;
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        row2.AddChild(nextDd);
        row2.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        var addEffectBtn = new Button { Text = "＋ Effect" };
        addEffectBtn.Pressed += () =>
        {
            line.Effects.Add(new ConversationEffect
            {
                EffectType = "AddItem",
                Parameters = new Dictionary<string, string> { ["itemId"] = "" },
            });
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        row2.AddChild(addEffectBtn);
        var addChoiceBtn = new Button { Text = "＋ Choice" };
        addChoiceBtn.Pressed += () =>
        {
            line.Choices.Add(new Choice
                { Id = EditorState.GenerateId("choice", line.Choices.Select(c => c.Id)), Text = "" });
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        row2.AddChild(addChoiceBtn);
        vbox.AddChild(row2);
        foreach (var effect in line.Effects.ToList())
            vbox.AddChild(BuildEffectRow(convId, line, effect));
        foreach (var choice in line.Choices)
            vbox.AddChild(BuildChoiceRow(convId, conv, line, choice));
        return panel;
    }
    // ── Choice row ────────────────────────────────────────────────────────────
    private Control BuildChoiceRow(string convId, Conversation conv, DialogueLine line, Choice choice)
    {
        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 2);
        var textRow = new HBoxContainer();
        textRow.AddThemeConstantOverride("separation", 4);
        textRow.AddChild(new Label { Text = "    ↳", CustomMinimumSize = new Vector2(32, 0) });
        var textEdit = new LineEdit
            { Text = choice.Text, SizeFlagsHorizontal = SizeFlags.ExpandFill,
              PlaceholderText = "Choice text" }.Ltr();
        textEdit.CaretColumn = textEdit.Text.Length;
        textEdit.TextSubmitted += v => { choice.Text = v; _state.UpdateDialogueLine(_eventId, convId, line); };
        textEdit.FocusExited   += () => { choice.Text = textEdit.Text; _state.UpdateDialogueLine(_eventId, convId, line); };
        textRow.AddChild(textEdit);
        textRow.AddChild(new Label { Text = "→" });
        var nextDd = new OptionButton { CustomMinimumSize = new Vector2(200, 0) };
        PopulateLineDropdownWithLabels(nextDd, conv, choice.NextDialogueLineId);
        nextDd.ItemSelected += _ =>
        {
            choice.NextDialogueLineId = DropdownHelper.GetSelectedId(nextDd);
            if (string.IsNullOrEmpty(choice.NextDialogueLineId)) choice.NextDialogueLineId = null;
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        textRow.AddChild(nextDd);
        var delBtn = new Button { Text = "🗑" };
        delBtn.Pressed += () =>
        {
            line.Choices.Remove(choice);
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        textRow.AddChild(delBtn);
        vbox.AddChild(textRow);
        foreach (var cond in choice.Conditions.ToList())
            vbox.AddChild(BuildConditionRow(convId, conv, line, choice, cond));
        var addCondRow = new HBoxContainer();
        addCondRow.AddChild(new Control { CustomMinimumSize = new Vector2(32, 0) });
        var addCondBtn = new Button { Text = "＋ Condition" };
        addCondBtn.Pressed += () =>
        {
            choice.Conditions.Add(new Condition
            {
                ConditionType = "ItemInInventory",
                Parameters    = new Dictionary<string, string> { ["itemId"] = "", ["negate"] = "false" },
            });
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        addCondRow.AddChild(addCondBtn);
        vbox.AddChild(addCondRow);
        return vbox;
    }
    // ── Effect row ────────────────────────────────────────────────────────────

    private Control BuildEffectRow(string convId, DialogueLine line, ConversationEffect effect)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 4);
        // Indent to align with choice/condition rows
        row.AddChild(new Label { Text = "  ⚡", CustomMinimumSize = new Vector2(32, 0) });

        // System dropdown — expandable for future systems
        var systemDd = new OptionButton { CustomMinimumSize = new Vector2(110, 0) };
        systemDd.AddItem("Inventory"); systemDd.SetItemMetadata(0, "inventory");
        systemDd.Selected = 0; // only inventory for now

        // Action dropdown — Add / Remove
        var actionDd = new OptionButton { CustomMinimumSize = new Vector2(120, 0) };
        actionDd.AddItem("Add item");    actionDd.SetItemMetadata(0, "AddItem");
        actionDd.AddItem("Remove item"); actionDd.SetItemMetadata(1, "RemoveItem");
        actionDd.Selected = effect.EffectType == "RemoveItem" ? 1 : 0;

        // Item target dropdown
        var itemDd = new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        effect.Parameters.TryGetValue("itemId", out var currentItemId);
        PopulateEffectTargets(itemDd, effect.EffectType, currentItemId);

        actionDd.ItemSelected += _ =>
        {
            effect.EffectType = (string)actionDd.GetItemMetadata(actionDd.Selected);
            // Keep itemId parameter — it applies to both Add and Remove
            PopulateEffectTargets(itemDd, effect.EffectType, currentItemId);
            _state.UpdateDialogueLine(_eventId, convId, line);
        };

        itemDd.ItemSelected += _ =>
        {
            effect.Parameters["itemId"] = DropdownHelper.GetSelectedId(itemDd);
            _state.UpdateDialogueLine(_eventId, convId, line);
        };

        var delBtn = new Button { Text = "🗑" };
        delBtn.Pressed += () =>
        {
            line.Effects.Remove(effect);
            _state.UpdateDialogueLine(_eventId, convId, line);
        };

        row.AddChild(systemDd);
        row.AddChild(actionDd);
        row.AddChild(itemDd);
        row.AddChild(delBtn);
        return row;
    }

    private void PopulateEffectTargets(OptionButton button, string effectType, string? selectedId)
    {
        button.Clear();
        button.AddItem("(select item…)");
        button.SetItemMetadata(0, DropdownHelper.NoneId);

        var idx = 1;
        foreach (var item in _state.Storybook.Items.Values)
        {
            button.AddItem(item.Name);
            button.SetItemMetadata(idx, item.Id);
            if (item.Id == selectedId) button.Selected = idx;
            idx++;
        }
    }

    // ── Condition row ─────────────────────────────────────────────────────────
    private Control BuildConditionRow(string convId, Conversation conv, DialogueLine line, Choice choice, Condition cond)
    {
        var row = new HBoxContainer();
        row.AddThemeConstantOverride("separation", 4);
        row.AddChild(new Control { CustomMinimumSize = new Vector2(48, 0) });
        var systemDd = new OptionButton { CustomMinimumSize = new Vector2(110, 0) };
        systemDd.AddItem("Inventory"); systemDd.SetItemMetadata(0, "ItemInInventory");
        systemDd.AddItem("Journal");   systemDd.SetItemMetadata(1, "InformationLearned");
        systemDd.Selected = cond.ConditionType == "InformationLearned" ? 1 : 0;
        var opDd = new OptionButton { CustomMinimumSize = new Vector2(160, 0) };
        opDd.AddItem("contains");         opDd.SetItemMetadata(0, "false");
        opDd.AddItem("does not contain"); opDd.SetItemMetadata(1, "true");
        var isNegate = cond.Parameters.TryGetValue("negate", out var negVal) && negVal == "true";
        opDd.Selected = isNegate ? 1 : 0;
        var itemKey = cond.ConditionType == "InformationLearned" ? "informationId" : "itemId";
        cond.Parameters.TryGetValue(itemKey, out var currentId);
        var itemDd = new OptionButton { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        PopulateConditionTargets(itemDd, cond.ConditionType, currentId);
        systemDd.ItemSelected += _ =>
        {
            cond.ConditionType = (string)systemDd.GetItemMetadata(systemDd.Selected);
            cond.Parameters.Clear();
            cond.Parameters["negate"] = (string)opDd.GetItemMetadata(opDd.Selected);
            var key = cond.ConditionType == "InformationLearned" ? "informationId" : "itemId";
            cond.Parameters[key] = "";
            PopulateConditionTargets(itemDd, cond.ConditionType, null);
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        opDd.ItemSelected += _ =>
        {
            cond.Parameters["negate"] = (string)opDd.GetItemMetadata(opDd.Selected);
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        itemDd.ItemSelected += _ =>
        {
            var key = cond.ConditionType == "InformationLearned" ? "informationId" : "itemId";
            cond.Parameters[key] = DropdownHelper.GetSelectedId(itemDd);
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        var delCondBtn = new Button { Text = "🗑" };
        delCondBtn.Pressed += () =>
        {
            choice.Conditions.Remove(cond);
            _state.UpdateDialogueLine(_eventId, convId, line);
        };
        row.AddChild(systemDd);
        row.AddChild(opDd);
        row.AddChild(itemDd);
        row.AddChild(delCondBtn);
        return row;
    }
    // ── Helpers ───────────────────────────────────────────────────────────────
    private static void PopulateLineDropdownWithLabels(
        OptionButton button, Conversation conv, string? selectedId, string? excludeId = null)
    {
        button.Clear();
        button.AddItem("(end conversation)");
        button.SetItemMetadata(0, DropdownHelper.NoneId);
        var idx = 1;
        foreach (var line in conv.DialogueLines.Values)
        {
            if (line.Id == excludeId) continue;
            var preview = string.IsNullOrWhiteSpace(line.Text)
                ? $"[{line.Id}]"
                : $"{line.Speaker}: {line.Text[..Math.Min(line.Text.Length, 40)]}";
            button.AddItem(preview);
            button.SetItemMetadata(idx, line.Id);
            if (line.Id == selectedId) button.Selected = idx;
            idx++;
        }
    }
    private void PopulateSpeakers(OptionButton button, string? selectedSpeaker)
    {
        button.Clear();
        button.AddItem("Player");
        button.SetItemMetadata(0, "Player");
        if (selectedSpeaker == "Player") button.Selected = 0;
        var idx = 1;
        foreach (var c in _state.Storybook.Characters.Values)
        {
            button.AddItem(c.Name);
            button.SetItemMetadata(idx, c.Name);
            if (c.Name == selectedSpeaker) button.Selected = idx;
            idx++;
        }
    }
    private void PopulateConditionTargets(OptionButton button, string conditionType, string? selectedId)
    {
        button.Clear();
        button.AddItem("(select\u2026)");
        button.SetItemMetadata(0, DropdownHelper.NoneId);
        var idx = 1;
        if (conditionType == "InformationLearned")
        {
            button.AddItem(selectedId ?? "");
            button.SetItemMetadata(1, selectedId ?? "");
            button.Selected = 1;
        }
        else
        {
            foreach (var item in _state.Storybook.Items.Values)
            {
                button.AddItem(item.Name);
                button.SetItemMetadata(idx, item.Id);
                if (item.Id == selectedId) button.Selected = idx;
                idx++;
            }
        }
    }
}
