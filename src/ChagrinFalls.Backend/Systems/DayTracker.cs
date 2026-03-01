namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Tracks the current day in the game. Days are numbered starting from 1.
/// Provides an API to advance days and query the current day.
/// </summary>
public class DayTracker
{
    private int _currentDay;

    /// <summary>
    /// Initializes the day tracker to day 1.
    /// </summary>
    public DayTracker()
    {
        _currentDay = 1;
    }

    /// <summary>
    /// Initializes the day tracker to a specific day.
    /// </summary>
    /// <param name="day">The starting day number (must be 1 or greater).</param>
    public DayTracker(int day)
    {
        if (day < 1)
            throw new ArgumentOutOfRangeException(nameof(day), "Day must be 1 or greater.");
        _currentDay = day;
    }

    /// <summary>
    /// Gets the current day number.
    /// </summary>
    public int CurrentDay => _currentDay;

    /// <summary>
    /// Returns the current day as a formatted string (e.g., "Day 1").
    /// </summary>
    public string GetDayString() => $"Day {_currentDay}";

    /// <summary>
    /// Advances to the next day.
    /// </summary>
    public void AdvanceDay()
    {
        _currentDay++;
    }

    /// <summary>
    /// Advances by a specified number of days.
    /// </summary>
    /// <param name="days">Number of days to advance (must be positive).</param>
    public void AdvanceDays(int days)
    {
        if (days < 0)
            throw new ArgumentOutOfRangeException(nameof(days), "Cannot advance by a negative number of days.");
        _currentDay += days;
    }

    /// <summary>
    /// Sets the day to a specific day number.
    /// </summary>
    /// <param name="day">The day number (must be 1 or greater).</param>
    public void SetDay(int day)
    {
        if (day < 1)
            throw new ArgumentOutOfRangeException(nameof(day), "Day must be 1 or greater.");
        _currentDay = day;
    }

    /// <summary>
    /// Resets the day tracker to day 1.
    /// </summary>
    public void Reset()
    {
        _currentDay = 1;
    }
}

