using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class PlayerJournalTests
{
    [Fact]
    public void HasLearned_ReturnsFalse_WhenJournalIsEmpty()
    {
        var journal = new PlayerJournal();
        Assert.False(journal.HasLearned("clue_1"));
    }

    [Fact]
    public void HasLearned_ReturnsTrue_AfterLearning()
    {
        var journal = new PlayerJournal();
        journal.LearnInformation("clue_1");
        Assert.True(journal.HasLearned("clue_1"));
    }

    [Fact]
    public void HasLearned_IsCaseInsensitive()
    {
        var journal = new PlayerJournal();
        journal.LearnInformation("Clue_1");
        Assert.True(journal.HasLearned("clue_1"));
        Assert.True(journal.HasLearned("CLUE_1"));
    }

    [Fact]
    public void HasLearned_ReturnsFalse_ForNullOrWhitespace()
    {
        var journal = new PlayerJournal();
        Assert.False(journal.HasLearned(null!));
        Assert.False(journal.HasLearned("   "));
    }

    [Fact]
    public void LearnInformation_Throws_ForNullOrWhitespace()
    {
        var journal = new PlayerJournal();
        Assert.Throws<ArgumentException>(() => journal.LearnInformation(null!));
        Assert.Throws<ArgumentException>(() => journal.LearnInformation("   "));
    }

    [Fact]
    public void GetAllLearnedInformation_ReturnsAllEntries()
    {
        var journal = new PlayerJournal();
        journal.LearnInformation("clue_1");
        journal.LearnInformation("clue_2");
        Assert.Equal(2, journal.GetAllLearnedInformation().Count);
    }

    [Fact]
    public void LearnInformation_IsDeduplicated()
    {
        var journal = new PlayerJournal();
        journal.LearnInformation("clue_1");
        journal.LearnInformation("clue_1");
        Assert.Single(journal.GetAllLearnedInformation());
    }
}
