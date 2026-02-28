using ChagrinFalls.Backend.Models;
using Godot;

namespace ChagrinFalls.StoryEditor.Scripts;

/// <summary>
/// Static helpers for populating <see cref="OptionButton"/> dropdowns from live
/// storybook data. IDs are stored in item metadata so forms never construct them.
/// </summary>
public static class DropdownHelper
{
    public const string NoneId = "";

    /// <summary>Fills <paramref name="button"/> with "(none)" + all characters.</summary>
    public static void PopulateCharacters(OptionButton button, Storybook storybook, string? selectedId = null)
    {
        button.Clear();
        button.AddItem("(none)");
        button.SetItemMetadata(0, NoneId);
        var idx = 1;
        foreach (var c in storybook.Characters.Values)
        {
            button.AddItem(c.Name);
            button.SetItemMetadata(idx, c.Id);
            if (c.Id == selectedId) button.Selected = idx;
            idx++;
        }
    }

    /// <summary>Fills <paramref name="button"/> with "(none)" + all items.</summary>
    public static void PopulateItems(OptionButton button, Storybook storybook, string? selectedId = null)
    {
        button.Clear();
        button.AddItem("(none)");
        button.SetItemMetadata(0, NoneId);
        var idx = 1;
        foreach (var item in storybook.Items.Values)
        {
            button.AddItem(item.Name);
            button.SetItemMetadata(idx, item.Id);
            if (item.Id == selectedId) button.Selected = idx;
            idx++;
        }
    }

    /// <summary>Fills <paramref name="button"/> with "(none)" + all locations.</summary>
    public static void PopulateLocations(OptionButton button, Storybook storybook, string? selectedId = null)
    {
        button.Clear();
        button.AddItem("(none)");
        button.SetItemMetadata(0, NoneId);
        var idx = 1;
        foreach (var loc in storybook.Locations.Values)
        {
            button.AddItem(loc.Name);
            button.SetItemMetadata(idx, loc.Id);
            if (loc.Id == selectedId) button.Selected = idx;
            idx++;
        }
    }

    /// <summary>
    /// Fills <paramref name="button"/> with "(none)" + all conversation events.
    /// </summary>
    public static void PopulateConversationEvents(OptionButton button, Storybook storybook, string? selectedId = null)
    {
        button.Clear();
        button.AddItem("(none)");
        button.SetItemMetadata(0, NoneId);
        var idx = 1;
        foreach (var evt in storybook.ConversationEvents.Values)
        {
            button.AddItem(evt.Id);
            button.SetItemMetadata(idx, evt.Id);
            if (evt.Id == selectedId) button.Selected = idx;
            idx++;
        }
    }

    /// <summary>
    /// Fills <paramref name="button"/> with "(end)" + all dialogue lines in <paramref name="conversation"/>.
    /// </summary>
    public static void PopulateDialogueLines(OptionButton button, Conversation conversation, string? selectedId = null)
    {
        button.Clear();
        button.AddItem("(end)");
        button.SetItemMetadata(0, NoneId);
        var idx = 1;
        foreach (var line in conversation.DialogueLines.Values)
        {
            var label = string.IsNullOrWhiteSpace(line.Text)
                ? line.Id
                : $"{line.Speaker}: {line.Text[..Math.Min(line.Text.Length, 40)]}…";
            button.AddItem(label);
            button.SetItemMetadata(idx, line.Id);
            if (line.Id == selectedId) button.Selected = idx;
            idx++;
        }
    }

    /// <summary>Returns the ID stored in the currently selected item's metadata.</summary>
    public static string GetSelectedId(OptionButton button) =>
        button.Selected >= 0 ? (string)button.GetItemMetadata(button.Selected) : NoneId;
}

