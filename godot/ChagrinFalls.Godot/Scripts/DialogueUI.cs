using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;
using Godot;

namespace ChagrinFalls.Godot.Scripts;

/// <summary>
/// Manages the dialogue UI — displaying speaker name, dialogue text, and player choice buttons.
/// Attach this script to the DialogueUI CanvasLayer node in DialogueUI.tscn.
/// </summary>
public partial class DialogueUI : CanvasLayer
{
    // ── Node references ───────────────────────────────────────────────────────

    private Label _speakerLabel = null!;
    private Label _dialogueLabel = null!;
    private VBoxContainer _choicesContainer = null!;
    private Button _advanceButton = null!;

    // ── State ─────────────────────────────────────────────────────────────────

    private DialogueManager? _dialogueManager;

    // ── Godot lifecycle ───────────────────────────────────────────────────────

    public override void _Ready()
    {
        // Get node references from the scene tree.
        _speakerLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/SpeakerLabel");
        _dialogueLabel = GetNode<Label>("Panel/MarginContainer/VBoxContainer/DialogueLabel");
        _choicesContainer = GetNode<VBoxContainer>("Panel/MarginContainer/VBoxContainer/ChoicesContainer");
        _advanceButton = GetNode<Button>("Panel/MarginContainer/VBoxContainer/AdvanceButton");

        Hide();
        _advanceButton.Pressed += OnAdvancePressed;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Initialises the UI with the given <see cref="DialogueManager"/> and starts displaying
    /// the conversation event immediately.
    /// </summary>
    public void StartConversation(DialogueManager manager, ConversationEvent conversationEvent)
    {
        _dialogueManager = manager;
        _dialogueManager.OnDialogueLineChanged += DisplayLine;
        _dialogueManager.OnConversationEnded   += OnConversationEnded;

        Show();
        _dialogueManager.StartEvent(conversationEvent);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void DisplayLine(DialogueLine line)
    {
        _speakerLabel.Text  = line.Speaker;
        _dialogueLabel.Text = line.Text;

        ClearChoices();

        var choices = _dialogueManager!.AvailableChoices;
        if (choices.Count > 0)
        {
            _advanceButton.Hide();
            for (var i = 0; i < choices.Count; i++)
            {
                var index  = i; // capture for lambda
                var button = new Button { Text = choices[i].Text };
                button.Pressed += () => OnChoicePressed(index);
                _choicesContainer.AddChild(button);
            }
        }
        else
        {
            _advanceButton.Show();
        }
    }

    private void ClearChoices()
    {
        foreach (Node child in _choicesContainer.GetChildren())
            child.QueueFree();
    }

    private void OnAdvancePressed()
    {
        _dialogueManager?.Advance();
    }

    private void OnChoicePressed(int index)
    {
        _dialogueManager?.SelectChoice(index);
    }

    private void OnConversationEnded()
    {
        if (_dialogueManager != null)
        {
            _dialogueManager.OnDialogueLineChanged -= DisplayLine;
            _dialogueManager.OnConversationEnded   -= OnConversationEnded;
            _dialogueManager = null;
        }

        Hide();
        EmitSignal(SignalName.ConversationFinished);
    }

    // ── Signals ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Emitted when the active conversation ends and the dialogue UI hides itself.
    /// </summary>
    [Signal]
    public delegate void ConversationFinishedEventHandler();
}
