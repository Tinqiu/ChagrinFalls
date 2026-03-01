namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Tracks the current time of day in the game. Time is represented in minutes from midnight.
/// Provides an API to advance time and query the current time of day.
/// </summary>
public class GameClock
{
    private int _minutesFromMidnight;

    /// <summary>
    /// Initializes the clock to midnight (00:00).
    /// </summary>
    public GameClock()
    {
        _minutesFromMidnight = 0;
    }

    /// <summary>
    /// Initializes the clock to a specific time.
    /// </summary>
    /// <param name="hours">Hour of the day (0-23).</param>
    /// <param name="minutes">Minutes within the hour (0-59).</param>
    public GameClock(int hours, int minutes)
    {
        if (hours < 0 || hours > 23)
            throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be between 0 and 23.");
        if (minutes < 0 || minutes > 59)
            throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be between 0 and 59.");

        _minutesFromMidnight = hours * 60 + minutes;
    }

    /// <summary>
    /// Gets the current hour (0-23).
    /// </summary>
    public int Hour => _minutesFromMidnight / 60;

    /// <summary>
    /// Gets the current minute within the hour (0-59).
    /// </summary>
    public int Minute => _minutesFromMidnight % 60;

    /// <summary>
    /// Gets the total minutes elapsed since midnight.
    /// </summary>
    public int MinutesFromMidnight => _minutesFromMidnight;

    /// <summary>
    /// Returns the current time as a formatted string (HH:MM).
    /// </summary>
    public string GetTimeString() => $"{Hour:D2}:{Minute:D2}";

    /// <summary>
    /// Advances the clock by a specified number of minutes.
    /// Time wraps around at midnight.
    /// </summary>
    /// <param name="minutes">Number of minutes to advance (can be negative to go backwards).</param>
    public void AdvanceTime(int minutes)
    {
        _minutesFromMidnight = (_minutesFromMidnight + minutes) % (24 * 60);
        // Handle negative wrap-around
        if (_minutesFromMidnight < 0)
            _minutesFromMidnight += 24 * 60;
    }

    /// <summary>
    /// Advances the clock by a specified number of hours and minutes.
    /// </summary>
    /// <param name="hours">Number of hours to advance.</param>
    /// <param name="minutes">Number of minutes to advance.</param>
    public void AdvanceTime(int hours, int minutes)
    {
        AdvanceTime(hours * 60 + minutes);
    }

    /// <summary>
    /// Sets the clock to a specific time.
    /// </summary>
    /// <param name="hours">Hour of the day (0-23).</param>
    /// <param name="minutes">Minutes within the hour (0-59).</param>
    public void SetTime(int hours, int minutes)
    {
        if (hours < 0 || hours > 23)
            throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be between 0 and 23.");
        if (minutes < 0 || minutes > 59)
            throw new ArgumentOutOfRangeException(nameof(minutes), "Minutes must be between 0 and 59.");

        _minutesFromMidnight = hours * 60 + minutes;
    }

    /// <summary>
    /// Resets the clock to midnight (00:00).
    /// </summary>
    public void Reset()
    {
        _minutesFromMidnight = 0;
    }
}

