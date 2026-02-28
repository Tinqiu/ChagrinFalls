using ChagrinFalls.Backend.Conditions;
using ChagrinFalls.Backend.Models;


namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Manages the flow of a <see cref="ConversationEvent"/>, advancing through dialogue lines,
/// evaluating conditions, and presenting available player choices.
/// </summary>
public class DialogueManager
{
    private readonly Dictionary<string, IConditionEvaluator> _evaluators;
    private readonly GameState _gameState;

    private ConversationEvent? _currentEvent;
    private Conversation? _currentConversation;
    private DialogueLine? _currentLine;

    /// <summary>
    /// Raised whenever the active dialogue line changes.
    /// </summary>
    public event Action<DialogueLine>? OnDialogueLineChanged;

    /// <summary>
    /// Raised when the active conversation ends (no more lines or choices).
    /// </summary>
    public event Action? OnConversationEnded;

    /// <param name="gameState">The current game state used when evaluating conditions.</param>
    /// <param name="evaluators">
    /// Optional additional condition evaluators. The built-in evaluators
    /// (<see cref="ItemInInventoryEvaluator"/> and <see cref="InformationLearnedEvaluator"/>)
    /// are always registered automatically.
    /// </param>
    public DialogueManager(GameState gameState, IEnumerable<IConditionEvaluator>? evaluators = null)
    {
        _gameState = gameState ?? throw new ArgumentNullException(nameof(gameState));

        _evaluators = new Dictionary<string, IConditionEvaluator>(StringComparer.OrdinalIgnoreCase);
        RegisterEvaluator(new ItemInInventoryEvaluator());
        RegisterEvaluator(new InformationLearnedEvaluator());

        if (evaluators != null)
        {
            foreach (var evaluator in evaluators)
                RegisterEvaluator(evaluator);
        }
    }

    /// <summary>
    /// Returns the dialogue line currently being displayed, or <c>null</c> when no conversation is active.
    /// </summary>
    public DialogueLine? CurrentLine => _currentLine;

    /// <summary>
    /// Returns the choices available for the current dialogue line, filtered by their conditions.
    /// Returns an empty list when no conversation is active or the line has no choices.
    /// </summary>
    public IReadOnlyList<Choice> AvailableChoices =>
        _currentLine?.Choices.Where(c => AllConditionsMet(c.Conditions)).ToList() ?? [];

    /// <summary>
    /// Returns whether a conversation is currently in progress.
    /// </summary>
    public bool IsConversationActive => _currentLine != null;

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Starts the given conversation event, beginning with its starting conversation and line.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the event conditions are not met, the starting conversation is missing,
    /// or the starting dialogue line is missing.
    /// </exception>
    public void StartEvent(ConversationEvent conversationEvent)
    {
        ArgumentNullException.ThrowIfNull(conversationEvent);

        if (!AllConditionsMet(conversationEvent.Conditions))
            throw new InvalidOperationException(
                $"Conditions for conversation event '{conversationEvent.Id}' are not met.");

        if (!conversationEvent.Conversations.TryGetValue(
                conversationEvent.StartingConversationId, out var conversation))
            throw new InvalidOperationException(
                $"Starting conversation '{conversationEvent.StartingConversationId}' not found in event '{conversationEvent.Id}'.");

        _currentEvent = conversationEvent;
        StartConversation(conversation);
    }

    /// <summary>
    /// Advances the conversation to the next dialogue line when no available choices are present.
    /// If the current line has at least one available (condition-satisfied) choice, use
    /// <see cref="SelectChoice"/> instead.
    /// </summary>
    public void Advance()
    {
        if (_currentLine == null) return;

        if (AvailableChoices.Count > 0)
            return; // Player must select a choice.

        NavigateToLine(_currentLine.NextDialogueLineId);
    }

    /// <summary>
    /// Selects a player choice by its index in <see cref="AvailableChoices"/> and advances
    /// the conversation to the line it points to.
    /// </summary>
    /// <param name="choiceIndex">Zero-based index into <see cref="AvailableChoices"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
    public void SelectChoice(int choiceIndex)
    {
        var choices = AvailableChoices;
        if (choiceIndex < 0 || choiceIndex >= choices.Count)
            throw new ArgumentOutOfRangeException(nameof(choiceIndex),
                $"Choice index {choiceIndex} is out of range. There are {choices.Count} available choices.");

        NavigateToLine(choices[choiceIndex].NextDialogueLineId);
    }

    /// <summary>
    /// Registers or replaces a condition evaluator for its declared <see cref="IConditionEvaluator.ConditionType"/>.
    /// </summary>
    public void RegisterEvaluator(IConditionEvaluator evaluator)
    {
        ArgumentNullException.ThrowIfNull(evaluator);
        _evaluators[evaluator.ConditionType] = evaluator;
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void StartConversation(Conversation conversation)
    {
        _currentConversation = conversation;

        if (!conversation.DialogueLines.TryGetValue(
                conversation.StartingDialogueLineId, out var startLine))
            throw new InvalidOperationException(
                $"Starting dialogue line '{conversation.StartingDialogueLineId}' not found in conversation '{conversation.Id}'.");

        SetCurrentLine(startLine);
    }

    private void NavigateToLine(string? nextLineId)
    {
        if (nextLineId == null)
        {
            EndConversation();
            return;
        }

        if (_currentConversation == null ||
            !_currentConversation.DialogueLines.TryGetValue(nextLineId, out var nextLine))
        {
            EndConversation();
            return;
        }

        // Skip lines whose conditions are not met, following their NextDialogueLineId chain.
        // Track visited IDs to break out of any cycle formed by gated lines.
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (nextLine != null && !AllConditionsMet(nextLine.Conditions))
        {
            if (!visited.Add(nextLine.Id))
            {
                // Cycle detected among condition-gated lines — end the conversation.
                EndConversation();
                return;
            }

            var skippedId = nextLine.NextDialogueLineId;
            nextLine = skippedId != null &&
                       _currentConversation.DialogueLines.TryGetValue(skippedId, out var skipped)
                ? skipped
                : null;
        }

        if (nextLine == null)
        {
            EndConversation();
            return;
        }

        SetCurrentLine(nextLine);
    }

    private void SetCurrentLine(DialogueLine line)
    {
        _currentLine = line;
        OnDialogueLineChanged?.Invoke(line);
    }

    private void EndConversation()
    {
        _currentLine = null;
        _currentConversation = null;
        _currentEvent = null;
        OnConversationEnded?.Invoke();
    }

    private bool AllConditionsMet(IEnumerable<Condition> conditions)
    {
        foreach (var condition in conditions)
        {
            if (!_evaluators.TryGetValue(condition.ConditionType, out var evaluator))
                return false; // Unknown condition type treated as unmet.

            if (!evaluator.Evaluate(condition, _gameState))
                return false;
        }
        return true;
    }
}
