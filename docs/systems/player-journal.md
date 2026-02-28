# Player Journal

`ChagrinFalls.Backend.Systems.PlayerJournal`

Tracks pieces of information (clues, facts, story beats) that the player has learned. Information IDs are compared case-insensitively. Accessed via `GameState.Journal`.

---

## Methods

### `LearnInformation`
```csharp
public void LearnInformation(string informationId)
```
Records a piece of information in the journal. Throws `ArgumentException` if `informationId` is null or whitespace. Learning the same ID twice is a no-op.

### `HasLearned`
```csharp
public bool HasLearned(string informationId)
```
Returns `true` if the player has already learned the specified information.

### `GetAllLearnedInformation`
```csharp
public IReadOnlyCollection<string> GetAllLearnedInformation()
```
Returns a snapshot of all information IDs currently recorded.

---

## Usage in conditions

The built-in `InformationLearned` condition evaluator checks the journal automatically. See the [Conditions](conditions.md) page for the parameter format.

---

## Example

```csharp
var journal = gameState.Journal;

journal.LearnInformation("victim_identity");
journal.LearnInformation("last_known_location");

Console.WriteLine(journal.HasLearned("victim_identity")); // true
Console.WriteLine(journal.HasLearned("murder_weapon"));   // false

foreach (var info in journal.GetAllLearnedInformation())
    Console.WriteLine(info);
```

