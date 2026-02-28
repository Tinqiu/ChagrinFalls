using ChagrinFalls.Backend.Systems;
using Godot;

namespace ChagrinFalls.Godot.Scripts;

/// <summary>
/// Overlay screen that displays the player's current inventory as a simple item list.
/// Sits on a high CanvasLayer so it renders above all other UI.
/// While visible, the layer blocks mouse input from reaching layers beneath it.
/// </summary>
public partial class InventoryScreen : CanvasLayer
{
    private VBoxContainer _itemList = null!;

    public override void _Ready()
    {
        GetNode<Button>("Overlay/Panel/VBoxContainer/TitleBar/CloseButton").Pressed += Close;
        Hide();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Populates the item list from <paramref name="inventory"/> and shows the screen.
    /// </summary>
    public void Open(PlayerInventory inventory)
    {
        _itemList = GetNode<VBoxContainer>("Overlay/Panel/VBoxContainer/ItemList");

        // Clear any previously displayed items.
        foreach (Node child in _itemList.GetChildren())
            child.QueueFree();

        var items = inventory.GetAllItems();
        if (items.Count == 0)
        {
            _itemList.AddChild(new Label { Text = "(empty)" });
        }
        else
        {
            foreach (var item in items)
                _itemList.AddChild(new Label { Text = item });
        }

        Show();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void Close()
    {
        Hide();
    }
}

