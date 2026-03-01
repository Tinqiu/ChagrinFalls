using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class GameClockTests
{
    [Fact]
    public void Constructor_DefaultsToMidnight()
    {
        var clock = new GameClock();
        Assert.Equal(0, clock.Hour);
        Assert.Equal(0, clock.Minute);
        Assert.Equal(0, clock.MinutesFromMidnight);
    }

    [Fact]
    public void Constructor_WithHoursAndMinutes()
    {
        var clock = new GameClock(14, 30);
        Assert.Equal(14, clock.Hour);
        Assert.Equal(30, clock.Minute);
        Assert.Equal(14 * 60 + 30, clock.MinutesFromMidnight);
    }

    [Fact]
    public void Constructor_ThrowsForInvalidHours()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameClock(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameClock(24, 0));
    }

    [Fact]
    public void Constructor_ThrowsForInvalidMinutes()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameClock(12, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new GameClock(12, 60));
    }

    [Fact]
    public void GetTimeString_FormatsCorrectly()
    {
        var clock = new GameClock(9, 5);
        Assert.Equal("09:05", clock.GetTimeString());

        var clock2 = new GameClock(23, 59);
        Assert.Equal("23:59", clock2.GetTimeString());
    }

    [Fact]
    public void AdvanceTime_ByMinutes()
    {
        var clock = new GameClock(12, 0);
        clock.AdvanceTime(30);
        Assert.Equal(12, clock.Hour);
        Assert.Equal(30, clock.Minute);
    }

    [Fact]
    public void AdvanceTime_ByHoursAndMinutes()
    {
        var clock = new GameClock(12, 0);
        clock.AdvanceTime(2, 30);
        Assert.Equal(14, clock.Hour);
        Assert.Equal(30, clock.Minute);
    }

    [Fact]
    public void AdvanceTime_WrapsAroundMidnight()
    {
        var clock = new GameClock(23, 30);
        clock.AdvanceTime(60);
        Assert.Equal(0, clock.Hour);
        Assert.Equal(30, clock.Minute);
    }

    [Fact]
    public void AdvanceTime_WrapsAroundMultipleDays()
    {
        var clock = new GameClock(22, 0);
        clock.AdvanceTime(120); // 2 hours
        Assert.Equal(0, clock.Hour);
        Assert.Equal(0, clock.Minute);
    }

    [Fact]
    public void AdvanceTime_NegativeWraps()
    {
        var clock = new GameClock(2, 0);
        clock.AdvanceTime(-120);
        Assert.Equal(0, clock.Hour);
        Assert.Equal(0, clock.Minute);
    }

    [Fact]
    public void SetTime_Changes()
    {
        var clock = new GameClock(10, 0);
        clock.SetTime(15, 45);
        Assert.Equal(15, clock.Hour);
        Assert.Equal(45, clock.Minute);
    }

    [Fact]
    public void SetTime_ThrowsForInvalidValues()
    {
        var clock = new GameClock();
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.SetTime(-1, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.SetTime(24, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.SetTime(12, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.SetTime(12, 60));
    }

    [Fact]
    public void Reset_ReturnToMidnight()
    {
        var clock = new GameClock(15, 30);
        clock.Reset();
        Assert.Equal(0, clock.Hour);
        Assert.Equal(0, clock.Minute);
    }
}

