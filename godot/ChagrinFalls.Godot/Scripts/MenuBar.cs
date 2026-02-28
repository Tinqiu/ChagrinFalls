using Godot;

namespace ChagrinFalls.Godot.Scripts;

/// <summary>
/// Sidebar menu bar. Exposes signals for each menu button so parent scenes
/// can respond without the bar needing to know about game state.
/// </summary>
public partial class MenuBar : CanvasLayer
{
    [Signal]
    public delegate void BackpackPressedEventHandler();

    [Signal]
    public delegate void JournalPressedEventHandler();

    public override void _Ready()
    {
        GetNode<Button>("Panel/VBoxContainer/BackpackButton").Pressed += () => EmitSignal(SignalName.BackpackPressed);
        GetNode<Button>("Panel/VBoxContainer/JournalButton").Pressed  += () => EmitSignal(SignalName.JournalPressed);
    }
}

