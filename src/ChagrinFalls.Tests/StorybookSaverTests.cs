using System.Text.Json;
using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;
namespace ChagrinFalls.Tests;
public class StorybookSaverTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private readonly StorybookLoader _loader = new();
    public StorybookSaverTests() => Directory.CreateDirectory(_tempDir);
    public void Dispose()        => Directory.Delete(_tempDir, recursive: true);
    private string TempPath(string fileName) => Path.Combine(_tempDir, fileName);
    
    // ── Argument validation ───────────────────────────────────────────────────
    [Fact]
    public void Save_Throws_WhenStorybookIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            StorybookSaver.Save(null!, TempPath("out.json")));
    }
    [Fact]
    public void Save_Throws_WhenFilePathIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            StorybookSaver.Save(new Storybook { Id = "x" }, null!));
    }
    [Fact]
    public void Save_Throws_WhenFilePathIsWhitespace()
    {
        Assert.Throws<ArgumentException>(() =>
            StorybookSaver.Save(new Storybook { Id = "x" }, "   "));
    }
    // ── File output ───────────────────────────────────────────────────────────
    [Fact]
    public void Save_CreatesFile_AtSpecifiedPath()
    {
        var path = TempPath("storybook.json");
        StorybookSaver.Save(MinimalStorybook(), path);
        Assert.True(File.Exists(path));
    }
    [Fact]
    public void Save_CreatesParentDirectory_WhenItDoesNotExist()
    {
        var path = TempPath(Path.Combine("nested", "dir", "storybook.json"));
        StorybookSaver.Save(MinimalStorybook(), path);
        Assert.True(File.Exists(path));
    }
    [Fact]
    public void Save_OverwritesExistingFile()
    {
        var path = TempPath("storybook.json");
        File.WriteAllText(path, "old content");
        StorybookSaver.Save(MinimalStorybook("new-id"), path);
        var json = File.ReadAllText(path);
        Assert.Contains("new-id", json);
        Assert.DoesNotContain("old content", json);
    }
    // ── JSON naming conventions ───────────────────────────────────────────────
    [Fact]
    public void Save_WritesCamelCasePropertyNames()
    {
        var path = TempPath("storybook.json");
        StorybookSaver.Save(MinimalStorybook(), path);
        var json = File.ReadAllText(path);
        var doc  = JsonDocument.Parse(json);
        Assert.True(doc.RootElement.TryGetProperty("id", out _),
            "Expected camelCase 'id' property");
        Assert.True(doc.RootElement.TryGetProperty("startingLocationId", out _),
            "Expected camelCase 'startingLocationId' property");
        Assert.True(doc.RootElement.TryGetProperty("conversationEvents", out _),
            "Expected camelCase 'conversationEvents' property");
        Assert.False(doc.RootElement.TryGetProperty("Id", out _),
            "PascalCase 'Id' must not appear");
    }
    [Fact]
    public void Save_WritesCamelCaseEnumValues_ForPointOfInterestType()
    {
        var storybook = MinimalStorybook();
        storybook.Locations["loc_a"] = new Location
        {
            Id   = "loc_a",
            Name = "Test Location",
            PointsOfInterest = new List<PointOfInterest>
            {
                new() { Id = "poi_1", Name = "An Item", Type = PointOfInterestType.Item },
                new() { Id = "poi_2", Name = "A Character", Type = PointOfInterestType.Character },
            }
        };
        var path = TempPath("enum.json");
        StorybookSaver.Save(storybook, path);
        var json = File.ReadAllText(path);
        Assert.Contains("\"item\"",      json, StringComparison.Ordinal);
        Assert.Contains("\"character\"", json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"Item\"",      json, StringComparison.Ordinal);
        Assert.DoesNotContain("\"Character\"", json, StringComparison.Ordinal);
    }
    [Fact]
    public void Save_WritesIndentedJson()
    {
        var path = TempPath("indented.json");
        StorybookSaver.Save(MinimalStorybook(), path);
        var json = File.ReadAllText(path);
        // Indented JSON always contains newlines
        Assert.Contains('\n', json);
    }
    // ── Round-trip: Save → Load ───────────────────────────────────────────────
    [Fact]
    public void RoundTrip_MinimalStorybook_PreservesMetadata()
    {
        var original = MinimalStorybook("round-trip-story", "Round Trip", "A test storybook.", "loc_start");
        var path     = TempPath("round-trip.json");
        StorybookSaver.Save(original, path);
        var loaded = _loader.LoadFile(path);
        Assert.Equal(original.Id,                 loaded.Id);
        Assert.Equal(original.Title,              loaded.Title);
        Assert.Equal(original.Description,        loaded.Description);
        Assert.Equal(original.StartingLocationId, loaded.StartingLocationId);
    }
    [Fact]
    public void RoundTrip_PreservesLocationsAndPointsOfInterest()
    {
        var storybook = MinimalStorybook();
        storybook.Locations["loc_a"] = new Location
        {
            Id   = "loc_a",
            Name = "Old Barn",
            PointsOfInterest = new List<PointOfInterest>
            {
                new()
                {
                    Id                  = "npc_farmer",
                    Name                = "Farmer Joe",
                    Type                = PointOfInterestType.Character,
                    CharacterId         = "farmer_joe",
                    ConversationEventId = "greet_evt",
                },
                new()
                {
                    Id     = "itm_key",
                    Name   = "Rusty Key",
                    Type   = PointOfInterestType.Item,
                    ItemId = "rusty_key",
                },
            }
        };
        var path = TempPath("locations.json");
        StorybookSaver.Save(storybook, path);
        var loaded = _loader.LoadFile(path);
        var loc = loaded.Locations["loc_a"];
        Assert.Equal("Old Barn", loc.Name);
        Assert.Equal(2, loc.PointsOfInterest.Count);
        var npc = loc.PointsOfInterest.Single(p => p.Id == "npc_farmer");
        Assert.Equal(PointOfInterestType.Character, npc.Type);
        Assert.Equal("greet_evt", npc.ConversationEventId);
        Assert.Equal("farmer_joe", npc.CharacterId);
        var key = loc.PointsOfInterest.Single(p => p.Id == "itm_key");
        Assert.Equal(PointOfInterestType.Item, key.Type);
        Assert.Equal("rusty_key", key.ItemId);
    }
    [Fact]
    public void RoundTrip_PreservesCharactersAndItems()
    {
        var storybook = MinimalStorybook();
        storybook.Characters["margery"] = new Character
            { Id = "margery", Name = "Margery", Description = "A suspicious baker." };
        storybook.Items["torn_letter"] = new Item
            { Id = "torn_letter", Name = "Torn Letter", Description = "An incriminating note." };
        var path = TempPath("chars-items.json");
        StorybookSaver.Save(storybook, path);
        var loaded = _loader.LoadFile(path);
        var c = loaded.Characters["margery"];
        Assert.Equal("Margery",            c.Name);
        Assert.Equal("A suspicious baker.", c.Description);
        var i = loaded.Items["torn_letter"];
        Assert.Equal("Torn Letter",            i.Name);
        Assert.Equal("An incriminating note.", i.Description);
    }
    [Fact]
    public void RoundTrip_PreservesConversationEventWithDialogueLines()
    {
        var storybook = MinimalStorybook();
        var line2 = new DialogueLine { Id = "line_2", Speaker = "Player", Text = "Thanks." };
        var line1 = new DialogueLine
        {
            Id                 = "line_1",
            Speaker            = "Margery",
            Text               = "Hello, Detective.",
            NextDialogueLineId = "line_2",
            Effects = new List<ConversationEffect>
            {
                new()
                {
                    EffectType = "AddItem",
                    Parameters = new Dictionary<string, string> { ["itemId"] = "clue_note" }
                }
            },
            Choices = new List<Choice>
            {
                new()
                {
                    Id                 = "choice_1",
                    Text               = "Tell me more.",
                    NextDialogueLineId = "line_2",
                    Conditions = new List<Condition>
                    {
                        new()
                        {
                            ConditionType = "ItemInInventory",
                            Parameters    = new Dictionary<string, string>
                                { ["itemId"] = "torn_letter", ["negate"] = "true" }
                        }
                    }
                }
            }
        };
        var conv = new Conversation
        {
            Id                     = "intro",
            StartingDialogueLineId = "line_1",
            DialogueLines          = new Dictionary<string, DialogueLine>
                { [line1.Id] = line1, [line2.Id] = line2 }
        };
        var evt = new ConversationEvent
        {
            Id                     = "margery_intro",
            StartingConversationId = "intro",
            Conversations          = new Dictionary<string, Conversation> { [conv.Id] = conv }
        };
        storybook.ConversationEvents[evt.Id] = evt;
        var path = TempPath("conversations.json");
        StorybookSaver.Save(storybook, path);
        var loaded = _loader.LoadFile(path);
        var loadedEvt  = loaded.ConversationEvents["margery_intro"];
        var loadedConv = loadedEvt.Conversations["intro"];
        Assert.Equal("line_1", loadedConv.StartingDialogueLineId);
        var loadedLine1 = loadedConv.DialogueLines["line_1"];
        Assert.Equal("Margery",  loadedLine1.Speaker);
        Assert.Equal("line_2",   loadedLine1.NextDialogueLineId);
        // Effect survived the round-trip
        var effect = Assert.Single(loadedLine1.Effects);
        Assert.Equal("AddItem",    effect.EffectType);
        Assert.Equal("clue_note",  effect.Parameters["itemId"]);
        // Choice and its condition survived
        var choice = Assert.Single(loadedLine1.Choices);
        Assert.Equal("Tell me more.", choice.Text);
        var cond = Assert.Single(choice.Conditions);
        Assert.Equal("ItemInInventory", cond.ConditionType);
        Assert.Equal("torn_letter",     cond.Parameters["itemId"]);
        Assert.Equal("true",            cond.Parameters["negate"]);
    }
    [Fact]
    public void RoundTrip_PreservesConditionalBranches()
    {
        var storybook = MinimalStorybook();
        var lineB = new DialogueLine { Id = "line_b", Speaker = "NPC", Text = "You have it!" };
        var lineA = new DialogueLine
        {
            Id                 = "line_a",
            Speaker            = "NPC",
            Text               = "Hmm...",
            NextDialogueLineId = null,
            Branches = new List<ConditionalBranch>
            {
                new()
                {
                    Id                 = "branch_1",
                    NextDialogueLineId = "line_b",
                    Conditions = new List<Condition>
                    {
                        new()
                        {
                            ConditionType = "ItemInInventory",
                            Parameters    = new Dictionary<string, string> { ["itemId"] = "magic_key" }
                        }
                    }
                }
            }
        };
        var conv = new Conversation
        {
            Id                     = "branch_conv",
            StartingDialogueLineId = "line_a",
            DialogueLines          = new Dictionary<string, DialogueLine>
                { [lineA.Id] = lineA, [lineB.Id] = lineB }
        };
        var evt = new ConversationEvent
        {
            Id                     = "branch_evt",
            StartingConversationId = "branch_conv",
            Conversations          = new Dictionary<string, Conversation> { [conv.Id] = conv }
        };
        storybook.ConversationEvents[evt.Id] = evt;
        var path = TempPath("branches.json");
        StorybookSaver.Save(storybook, path);
        var loaded = _loader.LoadFile(path);
        var loadedLine = loaded.ConversationEvents["branch_evt"]
                               .Conversations["branch_conv"]
                               .DialogueLines["line_a"];
        var branch = Assert.Single(loadedLine.Branches);
        Assert.Equal("branch_1", branch.Id);
        Assert.Equal("line_b",   branch.NextDialogueLineId);
        var cond = Assert.Single(branch.Conditions);
        Assert.Equal("ItemInInventory", cond.ConditionType);
        Assert.Equal("magic_key",       cond.Parameters["itemId"]);
    }
    // ── Private helpers ───────────────────────────────────────────────────────
    private static Storybook MinimalStorybook(
        string id          = "test-story",
        string title       = "Test Story",
        string description = "A test.",
        string startingLocationId = "loc_start") => new()
    {
        Id                 = id,
        Title              = title,
        Description        = description,
        StartingLocationId = startingLocationId,
    };
}
