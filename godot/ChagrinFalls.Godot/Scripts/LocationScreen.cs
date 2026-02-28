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

    private LocationManager _locationManager = null!;

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

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Binds this screen to a <see cref="LocationManager"/> and renders the current location.
    /// </summary>
    public void Initialise(LocationManager locationManager)
    {
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
