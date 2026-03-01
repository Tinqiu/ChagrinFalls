# Game Clock

`ChagrinFalls.Backend.Systems.GameClock`

Tracks the current time of day in the game using hours and minutes. Time wraps around at midnight (24-hour format). Accessed via `GameState.Clock`.

---

## Time Representation

Time is stored internally as minutes from midnight (00:00). A full day is 1440 minutes (24 × 60).

---

## Constructors

### Default Constructor
```csharp
public GameClock()
```
Initializes the clock to midnight (00:00).

### Constructor with Time
```csharp
public GameClock(int hours, int minutes)
```
Initializes the clock to a specific time. Throws `ArgumentOutOfRangeException` if hours are not 0-23 or minutes are not 0-59.

---

## Properties

### `Hour`
```csharp
public int Hour { get; }
```
Returns the current hour (0-23).

### `Minute`
```csharp
public int Minute { get; }
```
Returns the current minute within the hour (0-59).

### `MinutesFromMidnight`
```csharp
public int MinutesFromMidnight { get; }
```
Returns the total minutes elapsed since midnight.

---

## Methods

### `GetTimeString`
```csharp
public string GetTimeString()
```
Returns the current time as a formatted string (HH:MM format, e.g., "14:30").

### `AdvanceTime` (by minutes)
```csharp
public void AdvanceTime(int minutes)
```
Advances the clock by a specified number of minutes. Time wraps around at midnight. Can accept negative values to go backwards.

### `AdvanceTime` (by hours and minutes)
```csharp
public void AdvanceTime(int hours, int minutes)
```
Advances the clock by a specified number of hours and minutes.

### `SetTime`
```csharp
public void SetTime(int hours, int minutes)
```
Sets the clock to a specific time. Throws `ArgumentOutOfRangeException` if values are invalid.

### `Reset`
```csharp
public void Reset()
```
Resets the clock to midnight (00:00).

---

## Example

```csharp
var clock = gameState.Clock;

// Start at 9:30 AM
clock.SetTime(9, 30);
Console.WriteLine(clock.GetTimeString()); // "09:30"

// Advance by 2 hours and 45 minutes
clock.AdvanceTime(2, 45);
Console.WriteLine(clock.GetTimeString()); // "12:15"

// Check the current hour
Console.WriteLine($"Current hour: {clock.Hour}"); // 12

// Advance by 720 minutes (12 hours) — wraps to midnight
clock.AdvanceTime(720);
Console.WriteLine(clock.GetTimeString()); // "00:15"
```

