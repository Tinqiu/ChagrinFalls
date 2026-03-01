﻿namespace ChagrinFalls.Backend.Models;

/// <summary>
/// A self-contained storyline — bundles all locations, conversation events,
/// and metadata needed to run one complete story arc.
/// Loaded from a JSON file on disk by <see cref="Systems.StorybookLoader"/>.
/// </summary>
public class Storybook
{
    /// <summary>Unique identifier for this storybook (e.g. "margery-and-the-spooky-shack").</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Display title shown to the player on the storybook selection screen.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Short description shown alongside the title on the selection screen.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>The ID of the location the player starts in when this storybook is loaded.
    /// Must match a key in <see cref="Locations"/>.
    /// </summary>
    public string StartingLocationId { get; set; } = string.Empty;

    /// <summary>The starting day for this storybook (default: 1).</summary>
    public int InitialDay { get; set; } = 1;

    /// <summary>The starting hour for this storybook (0-23, default: 8).</summary>
    public int InitialHour { get; set; } = 8;

    /// <summary>The starting minute for this storybook (0-59, default: 0).</summary>
    public int InitialMinute { get; set; } = 0;

    /// <summary>All locations in this storybook, keyed by location ID.</summary>
    public Dictionary<string, Location> Locations { get; set; } = new();

    /// <summary>All characters in this storybook, keyed by character ID.</summary>
    public Dictionary<string, Character> Characters { get; set; } = new();

    /// <summary>All items in this storybook, keyed by item ID.</summary>
    public Dictionary<string, Item> Items { get; set; } = new();

    /// <summary>All conversation events in this storybook, keyed by event ID.</summary>
    public Dictionary<string, ConversationEvent> ConversationEvents { get; set; } = new();
}

