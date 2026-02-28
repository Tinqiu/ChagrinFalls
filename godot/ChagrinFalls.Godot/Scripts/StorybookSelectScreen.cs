using ChagrinFalls.Backend.Models;
using Godot;

namespace ChagrinFalls.Godot.Scripts;

/// <summary>
/// Full-screen overlay shown at startup. Lists available storybooks and emits
/// <see cref="StorybookSelected"/> when the player chooses one.
/// </summary>
public partial class StorybookSelectScreen : CanvasLayer
{
    private VBoxContainer _storybookList = null!;

    [Signal]
    public delegate void StorybookSelectedEventHandler(string storybookId);

    public override void _Ready()
    {
        _storybookList = GetNode<VBoxContainer>("Background/Panel/VBoxContainer/StorybookList");
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Populates the list with the given storybooks and shows the screen.
    /// </summary>
    public void Populate(IReadOnlyList<Storybook> storybooks)
    {
        foreach (Node child in _storybookList.GetChildren())
            child.QueueFree();

        foreach (var storybook in storybooks)
        {
            var captured = storybook;

            var button = new Button
            {
                Text          = storybook.Title,
                TooltipText   = storybook.Description,
                CustomMinimumSize = new Vector2(0, 56),
            };
            button.Pressed += () => OnStorybookPressed(captured);
            _storybookList.AddChild(button);
        }

        Show();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void OnStorybookPressed(Storybook storybook)
    {
        Hide();
        EmitSignal(SignalName.StorybookSelected, storybook.Id);
    }
}

