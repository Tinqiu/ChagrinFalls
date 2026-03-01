# Player Journal

`ChagrinFalls.Backend.Systems.PlayerJournal`

Tracks pieces of information (clues, facts, story beats) that the player has learned as journal entries. Each entry contains an ID, title, and description. Information IDs are compared case-insensitively. Accessed via `GameState.Journal`.

---

## JournalEntry

Each entry in the journal has three properties:

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string` | Unique identifier for the entry (e.g., `"victim_identity"`) |
| `Title` | `string` | Display title shown to the player (e.g., `"Victim Identified"`) |
| `Description` | `string` | The content or details of the entry |

---

## Methods

### `LearnInformation`
```csharp
public void LearnInformation(JournalEntry entry)
```
Records a journal entry in the journal. Throws `ArgumentNullException` if `entry` is null, and `ArgumentException` if the entry's ID is null or whitespace. Learning an entry with the same ID twice overwrites the previous entry.

### `HasLearned`
```csharp
public bool HasLearned(string? informationId)
```
Returns `true` if the player has already learned information with the specified ID.

### `GetAllLearnedInformation`
```csharp
public IReadOnlyCollection<JournalEntry> GetAllLearnedInformation()
```
Returns a snapshot of all journal entries currently recorded.

### `GetJournalEntry`
```csharp
public JournalEntry? GetJournalEntry(string? informationId)
```
Returns a specific journal entry by ID, or `null` if not found.

---

## Usage in conditions

The built-in `InformationLearned` condition evaluator checks the journal automatically using entry IDs. See the [Conditions](conditions.md) page for the parameter format.

---

## Example

```csharp
var journal = gameState.Journal;

var entry1 = new JournalEntry
{
    Id = "victim_identity",
    Title = "Victim Identified",
    Description = "The victim has been identified as Marcus Webb."
};

var entry2 = new JournalEntry
{
    Id = "last_known_location",
    Title = "Last Known Location",
    Description = "Marcus was last seen near the old mill on the outskirts of town."
};

journal.LearnInformation(entry1);
journal.LearnInformation(entry2);

Console.WriteLine(journal.HasLearned("victim_identity")); // true
Console.WriteLine(journal.HasLearned("murder_weapon"));   // false

var victimEntry = journal.GetJournalEntry("victim_identity");
Console.WriteLine(victimEntry?.Title);       // "Victim Identified"
Console.WriteLine(victimEntry?.Description); // "The victim has been identified as Marcus Webb."

foreach (var entry in journal.GetAllLearnedInformation())
    Console.WriteLine($"{entry.Title}: {entry.Description}");
```

