# Day Tracker

`ChagrinFalls.Backend.Systems.DayTracker`

Tracks the current day in the game. Days are numbered starting from 1. Accessed via `GameState.DayTracker`.

---

## Constructors

### Default Constructor
```csharp
public DayTracker()
```
Initializes the day tracker to day 1.

### Constructor with Starting Day
```csharp
public DayTracker(int day)
```
Initializes the day tracker to a specific day number. Throws `ArgumentOutOfRangeException` if the day is less than 1.

---

## Properties

### `CurrentDay`
```csharp
public int CurrentDay { get; }
```
Returns the current day number (always 1 or greater).

---

## Methods

### `GetDayString`
```csharp
public string GetDayString()
```
Returns the current day as a formatted string (e.g., "Day 1", "Day 10").

### `AdvanceDay`
```csharp
public void AdvanceDay()
```
Advances to the next day (increments by 1).

### `AdvanceDays`
```csharp
public void AdvanceDays(int days)
```
Advances by a specified number of days. Throws `ArgumentOutOfRangeException` if the number of days is negative.

### `SetDay`
```csharp
public void SetDay(int day)
```
Sets the day to a specific day number. Throws `ArgumentOutOfRangeException` if the day is less than 1.

### `Reset`
```csharp
public void Reset()
```
Resets the day tracker to day 1.

---

## Example

```csharp
var tracker = gameState.DayTracker;

// Start on day 1 (default)
Console.WriteLine(tracker.GetDayString()); // "Day 1"

// Advance to the next day
tracker.AdvanceDay();
Console.WriteLine(tracker.CurrentDay); // 2

// Advance by multiple days
tracker.AdvanceDays(3);
Console.WriteLine(tracker.GetDayString()); // "Day 5"

// Jump to a specific day
tracker.SetDay(15);
Console.WriteLine(tracker.GetDayString()); // "Day 15"

// Reset to day 1
tracker.Reset();
Console.WriteLine(tracker.CurrentDay); // 1
```

