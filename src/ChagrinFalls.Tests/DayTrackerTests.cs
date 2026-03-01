using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class DayTrackerTests
{
    [Fact]
    public void Constructor_DefaultsToDay1()
    {
        var tracker = new DayTracker();
        Assert.Equal(1, tracker.CurrentDay);
    }

    [Fact]
    public void Constructor_WithSpecificDay()
    {
        var tracker = new DayTracker(5);
        Assert.Equal(5, tracker.CurrentDay);
    }

    [Fact]
    public void Constructor_ThrowsForDay0OrNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new DayTracker(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new DayTracker(-1));
    }

    [Fact]
    public void GetDayString_FormatsCorrectly()
    {
        var tracker = new DayTracker(1);
        Assert.Equal("Day 1", tracker.GetDayString());

        var tracker2 = new DayTracker(10);
        Assert.Equal("Day 10", tracker2.GetDayString());
    }

    [Fact]
    public void AdvanceDay_IncrementsByOne()
    {
        var tracker = new DayTracker(3);
        tracker.AdvanceDay();
        Assert.Equal(4, tracker.CurrentDay);
    }

    [Fact]
    public void AdvanceDays_IncrementsMultiple()
    {
        var tracker = new DayTracker(5);
        tracker.AdvanceDays(3);
        Assert.Equal(8, tracker.CurrentDay);
    }

    [Fact]
    public void AdvanceDays_ThrowsForNegative()
    {
        var tracker = new DayTracker();
        Assert.Throws<ArgumentOutOfRangeException>(() => tracker.AdvanceDays(-1));
    }

    [Fact]
    public void SetDay_Changes()
    {
        var tracker = new DayTracker(5);
        tracker.SetDay(10);
        Assert.Equal(10, tracker.CurrentDay);
    }

    [Fact]
    public void SetDay_ThrowsForDay0OrNegative()
    {
        var tracker = new DayTracker();
        Assert.Throws<ArgumentOutOfRangeException>(() => tracker.SetDay(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => tracker.SetDay(-1));
    }

    [Fact]
    public void Reset_ReturnToDay1()
    {
        var tracker = new DayTracker(10);
        tracker.Reset();
        Assert.Equal(1, tracker.CurrentDay);
    }
}

