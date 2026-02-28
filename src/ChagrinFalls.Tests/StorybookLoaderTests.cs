using System.Text.Json;
using ChagrinFalls.Backend.Models;
using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class StorybookLoaderTests : IDisposable
{
    // Each test gets its own temp directory, cleaned up in Dispose().
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    private readonly StorybookLoader _loader = new();

    public StorybookLoaderTests() => Directory.CreateDirectory(_tempDir);
    public void Dispose()        => Directory.Delete(_tempDir, recursive: true);

    // ── Helpers ───────────────────────────────────────────────────────────────

    private string WriteJson(string fileName, string json)
    {
        var path = Path.Combine(_tempDir, fileName);
        File.WriteAllText(path, json);
        return path;
    }

    private string WriteStorybook(string fileName, string id = "test-story",
        string title = "Test Story", string startingLocationId = "loc_a") =>
        WriteJson(fileName, $$"""
        {
          "id": "{{id}}",
          "title": "{{title}}",
          "description": "A test.",
          "startingLocationId": "{{startingLocationId}}",
          "locations": {},
          "conversationEvents": {}
        }
        """);

    // ── LoadFile — validation ─────────────────────────────────────────────────

    [Fact]
    public void LoadFile_Throws_WhenFilePathIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _loader.LoadFile(null!));
    }

    [Fact]
    public void LoadFile_Throws_WhenFilePathIsWhitespace()
    {
        Assert.Throws<ArgumentException>(() => _loader.LoadFile("   "));
    }

    [Fact]
    public void LoadFile_Throws_WhenFileDoesNotExist()
    {
        Assert.Throws<FileNotFoundException>(() =>
            _loader.LoadFile(Path.Combine(_tempDir, "nonexistent.json")));
    }

    // ── LoadFile — JSON errors ────────────────────────────────────────────────

    [Fact]
    public void LoadFile_Throws_WhenJsonIsInvalid()
    {
        var path = WriteJson("bad.json", "this is not json {{{");
        Assert.Throws<JsonException>(() => _loader.LoadFile(path));
    }

    [Fact]
    public void LoadFile_Throws_WhenJsonDeservesToNull()
    {
        var path = WriteJson("null.json", "null");
        Assert.Throws<JsonException>(() => _loader.LoadFile(path));
    }

    // ── LoadFile — business rule: Id must be present ──────────────────────────

    [Fact]
    public void LoadFile_Throws_WhenStorybookIdIsMissing()
    {
        var path = WriteJson("no-id.json", """
        {
          "id": "",
          "title": "No ID",
          "startingLocationId": "loc_a",
          "locations": {},
          "conversationEvents": {}
        }
        """);
        Assert.Throws<InvalidOperationException>(() => _loader.LoadFile(path));
    }

    [Fact]
    public void LoadFile_Throws_WhenStorybookIdIsWhitespace()
    {
        var path = WriteJson("whitespace-id.json", """
        {
          "id": "   ",
          "title": "Whitespace ID",
          "startingLocationId": "loc_a",
          "locations": {},
          "conversationEvents": {}
        }
        """);
        Assert.Throws<InvalidOperationException>(() => _loader.LoadFile(path));
    }

    // ── LoadFile — happy path ─────────────────────────────────────────────────

    [Fact]
    public void LoadFile_ReturnsStorybook_WithCorrectMetadata()
    {
        var path = WriteStorybook("story.json", id: "my-story", title: "My Story");

        var storybook = _loader.LoadFile(path);

        Assert.Equal("my-story", storybook.Id);
        Assert.Equal("My Story", storybook.Title);
        Assert.Equal("A test.", storybook.Description);
    }

    [Fact]
    public void LoadFile_Deserialises_LocationsAndConversationEvents()
    {
        var path = WriteJson("full.json", """
        {
          "id": "full-story",
          "title": "Full Story",
          "description": "",
          "startingLocationId": "loc_a",
          "locations": {
            "loc_a": {
              "id": "loc_a",
              "name": "Location A",
              "pointsOfInterest": []
            }
          },
          "conversationEvents": {
            "evt_1": {
              "id": "evt_1",
              "conditions": [],
              "participants": [],
              "startingConversationId": "conv_1",
              "conversations": {}
            }
          }
        }
        """);

        var storybook = _loader.LoadFile(path);

        Assert.Single(storybook.Locations);
        Assert.Single(storybook.ConversationEvents);
        Assert.Equal("loc_a", storybook.Locations["loc_a"].Id);
        Assert.Equal("evt_1", storybook.ConversationEvents["evt_1"].Id);
    }

    [Fact]
    public void LoadFile_Deserialises_PointOfInterest_ItemType()
    {
        var path = WriteJson("poi.json", """
        {
          "id": "poi-story",
          "title": "POI Story",
          "description": "",
          "startingLocationId": "loc_a",
          "locations": {
            "loc_a": {
              "id": "loc_a",
              "name": "Location A",
              "pointsOfInterest": [
                { "id": "poi_1", "name": "Key", "type": "item", "itemId": "brass_key" }
              ]
            }
          },
          "conversationEvents": {}
        }
        """);

        var storybook = _loader.LoadFile(path);
        var poi = storybook.Locations["loc_a"].PointsOfInterest.Single();

        Assert.Equal(PointOfInterestType.Item, poi.Type);
        Assert.Equal("brass_key", poi.ItemId);
    }

    [Fact]
    public void LoadFile_Deserialises_PointOfInterest_CharacterType()
    {
        var path = WriteJson("char.json", """
        {
          "id": "char-story",
          "title": "Char Story",
          "description": "",
          "startingLocationId": "loc_a",
          "locations": {
            "loc_a": {
              "id": "loc_a",
              "name": "Location A",
              "pointsOfInterest": [
                { "id": "npc_1", "name": "Margery", "type": "character", "conversationEventId": "evt_1" }
              ]
            }
          },
          "conversationEvents": {}
        }
        """);

        var storybook = _loader.LoadFile(path);
        var poi = storybook.Locations["loc_a"].PointsOfInterest.Single();

        Assert.Equal(PointOfInterestType.Character, poi.Type);
        Assert.Equal("evt_1", poi.ConversationEventId);
    }

    // ── LoadAll — validation ──────────────────────────────────────────────────

    [Fact]
    public void LoadAll_Throws_WhenDirectoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _loader.LoadAll(null!));
    }

    [Fact]
    public void LoadAll_Throws_WhenDirectoryIsWhitespace()
    {
        Assert.Throws<ArgumentException>(() => _loader.LoadAll("   "));
    }

    // ── LoadAll — missing / empty directory ───────────────────────────────────

    [Fact]
    public void LoadAll_ReturnsEmpty_WhenDirectoryDoesNotExist()
    {
        var result = _loader.LoadAll(Path.Combine(_tempDir, "nonexistent"));
        Assert.Empty(result);
    }

    [Fact]
    public void LoadAll_ReturnsEmpty_WhenDirectoryContainsNoJsonFiles()
    {
        File.WriteAllText(Path.Combine(_tempDir, "readme.txt"), "hello");

        var result = _loader.LoadAll(_tempDir);
        Assert.Empty(result);
    }

    // ── LoadAll — happy path ──────────────────────────────────────────────────

    [Fact]
    public void LoadAll_ReturnsAllValidStorybooks()
    {
        WriteStorybook("story-a.json", id: "story-a");
        WriteStorybook("story-b.json", id: "story-b");

        var result = _loader.LoadAll(_tempDir);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Id == "story-a");
        Assert.Contains(result, s => s.Id == "story-b");
    }

    // ── LoadAll — error handling ──────────────────────────────────────────────

    [Fact]
    public void LoadAll_SkipsInvalidFiles_AndInvokesOnError()
    {
        WriteStorybook("good.json", id: "good-story");
        WriteJson("bad.json", "not valid json");

        var errors = new List<(string file, Exception ex)>();
        var result = _loader.LoadAll(_tempDir, (file, ex) => errors.Add((file, ex)));

        Assert.Single(result);
        Assert.Equal("good-story", result[0].Id);
        Assert.Single(errors);
        Assert.Contains("bad.json", errors[0].file);
    }

    [Fact]
    public void LoadAll_ReportsAllErrors_WhenAllFilesAreInvalid()
    {
        WriteJson("bad1.json", "{}");       // deserialises to empty id → InvalidOperationException
        WriteJson("bad2.json", "oops");     // JsonException

        var errors = new List<string>();
        var result = _loader.LoadAll(_tempDir, (file, _) => errors.Add(file));

        Assert.Empty(result);
        Assert.Equal(2, errors.Count);
    }

    [Fact]
    public void LoadAll_DoesNotInvokeOnError_WhenAllFilesAreValid()
    {
        WriteStorybook("story.json", id: "story");

        var errorInvoked = false;
        _loader.LoadAll(_tempDir, (_, _) => errorInvoked = true);

        Assert.False(errorInvoked);
    }
}

