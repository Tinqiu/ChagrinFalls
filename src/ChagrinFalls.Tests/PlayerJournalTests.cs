using ChagrinFalls.Backend.Models;
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
        var entry = new JournalEntry { Id = "clue_1", Title = "First Clue", Description = "Found a clue" };
        journal.LearnInformation(entry);
        Assert.True(journal.HasLearned("clue_1"));
    }

    [Fact]
    public void HasLearned_IsCaseInsensitive()
    {
        var journal = new PlayerJournal();
        var entry = new JournalEntry { Id = "Clue_1", Title = "First Clue", Description = "Found a clue" };
        journal.LearnInformation(entry);
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
    public void LearnInformation_Throws_ForNullEntry()
    {
        var journal = new PlayerJournal();
        Assert.Throws<ArgumentNullException>(() => journal.LearnInformation(null!));
    }

    [Fact]
    public void LearnInformation_Throws_ForNullOrWhitespaceId()
    {
        var journal = new PlayerJournal();
        var entry = new JournalEntry { Id = "", Title = "Test", Description = "Test" };
        Assert.Throws<ArgumentException>(() => journal.LearnInformation(entry));
    }

    [Fact]
    public void GetAllLearnedInformation_ReturnsAllEntries()
    {
        var journal = new PlayerJournal();
        var entry1 = new JournalEntry { Id = "clue_1", Title = "First Clue", Description = "Found a clue" };
        var entry2 = new JournalEntry { Id = "clue_2", Title = "Second Clue", Description = "Found another clue" };
        journal.LearnInformation(entry1);
        journal.LearnInformation(entry2);
        Assert.Equal(2, journal.GetAllLearnedInformation().Count);
    }

    [Fact]
    public void LearnInformation_IsDeduplicated()
    {
        var journal = new PlayerJournal();
        var entry = new JournalEntry { Id = "clue_1", Title = "First Clue", Description = "Found a clue" };
        journal.LearnInformation(entry);
        journal.LearnInformation(entry);
        Assert.Single(journal.GetAllLearnedInformation());
    }

    [Fact]
    public void GetJournalEntry_ReturnsEntry_WhenFound()
    {
        var journal = new PlayerJournal();
        var entry = new JournalEntry { Id = "clue_1", Title = "First Clue", Description = "Found a clue" };
        journal.LearnInformation(entry);
        var retrieved = journal.GetJournalEntry("clue_1");
        Assert.NotNull(retrieved);
        Assert.Equal("First Clue", retrieved.Title);
        Assert.Equal("Found a clue", retrieved.Description);
    }

    [Fact]
    public void GetJournalEntry_ReturnsNull_WhenNotFound()
    {
        var journal = new PlayerJournal();
        var retrieved = journal.GetJournalEntry("nonexistent");
        Assert.Null(retrieved);
    }
}
