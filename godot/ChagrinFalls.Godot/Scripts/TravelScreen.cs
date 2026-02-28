using ChagrinFalls.Backend.Systems;
using Godot;

namespace ChagrinFalls.Godot.Scripts;

/// <summary>
/// Modal overlay that lists all locations the player can travel to (excluding
/// the current one). Closes itself after the player selects a destination.
/// </summary>
public partial class TravelScreen : CanvasLayer
{
    private Label _titleLabel = null!;
    private VBoxContainer _locationList = null!;

    private LocationManager _locationManager = null!;

    public override void _Ready()
    {
        GetNode<Button>("Overlay/Panel/VBoxContainer/TitleBar/CloseButton").Pressed += Close;
        _titleLabel    = GetNode<Label>("Overlay/Panel/VBoxContainer/TitleBar/Title");
        _locationList  = GetNode<VBoxContainer>("Overlay/Panel/VBoxContainer/LocationList");
        Hide();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Populates the location list and shows the screen.
    /// </summary>
    public void Open(LocationManager locationManager)
    {
        _locationManager = locationManager;

        foreach (Node child in _locationList.GetChildren())
            child.QueueFree();

        var current = _locationManager.CurrentLocation;
        foreach (var location in _locationManager.AllLocations)
        {
            if (location.Id == current.Id)
                continue;

            var captured = location;
            var button   = new Button { Text = location.Name };
            button.Pressed += () =>
            {
                _locationManager.TravelTo(captured.Id);
                Close();
            };
            _locationList.AddChild(button);
        }

        Show();
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void Close() => Hide();
}

