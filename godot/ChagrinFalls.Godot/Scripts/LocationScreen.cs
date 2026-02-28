using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;
using Godot;

namespace ChagrinFalls.Godot.Scripts;

/// <summary>
/// Displays the current location name and its points of interest as interactive buttons.
/// Emits <see cref="CharacterInteracted"/> when the player clicks a character POI,
/// and calls <see cref="LocationManager.PickUpItem"/> directly for item POIs.
/// </summary>
public partial class LocationScreen : Control
{
    private Label _locationNameLabel = null!;
    private VBoxContainer _poiContainer = null!;

    private LocationManager? _locationManager;

    /// <summary>
    /// Emitted when the player clicks a character POI.
    /// The argument is the ID of the <see cref="PointOfInterest"/> that was clicked.
    /// </summary>
    [Signal]
    public delegate void CharacterInteractedEventHandler(string poiId);

    public override void _Ready()
    {
        _locationNameLabel = GetNode<Label>("VBoxContainer/LocationNameLabel");
        _poiContainer      = GetNode<VBoxContainer>("VBoxContainer/PoiContainer");
    }

    public override void _ExitTree()
    {
        if (_locationManager != null)
            _locationManager.OnLocationChanged -= Refresh;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Binds this screen to a <see cref="LocationManager"/> and renders the current location.
    /// Safe to call only once; subsequent calls are ignored.
    /// </summary>
    public void Initialise(LocationManager locationManager)
    {
        ArgumentNullException.ThrowIfNull(locationManager);

        if (_locationManager != null)
        {
            GD.PushWarning($"{nameof(LocationScreen)}.{nameof(Initialise)} called more than once — ignoring.");
            return;
        }

        _locationManager = locationManager;
        _locationManager.OnLocationChanged += Refresh;
        Refresh(_locationManager.CurrentLocation);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void Refresh(Location location)
    {
        _locationNameLabel.Text = location.Name;

        foreach (Node child in _poiContainer.GetChildren())
            child.QueueFree();

        foreach (var poi in location.PointsOfInterest)
        {
            var button = new Button { Text = poi.Name };
            var capturedPoi = poi;

            button.Pressed += () => OnPoiPressed(capturedPoi);
            _poiContainer.AddChild(button);
        }
    }

    private void OnPoiPressed(PointOfInterest poi)
    {
        if (_locationManager is null) return;

        switch (poi.Type)
        {
            case PointOfInterestType.Character:
                EmitSignal(SignalName.CharacterInteracted, poi.Id);
                break;

            case PointOfInterestType.Item:
                _locationManager.PickUpItem(poi.Id);
                break;
        }
    }
}
