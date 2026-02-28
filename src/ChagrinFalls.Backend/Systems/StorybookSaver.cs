using System.Text.Json;
using System.Text.Json.Serialization;
using ChagrinFalls.Backend.Models;

namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Serialises a <see cref="Storybook"/> to a JSON file on disk.
/// Produces output compatible with <see cref="StorybookLoader"/> (camelCase properties, camelCase enum values, indented).
/// </summary>
public class StorybookSaver
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy        = JsonNamingPolicy.CamelCase,
        WriteIndented               = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    /// <summary>
    /// Serialises <paramref name="storybook"/> and writes it to <paramref name="filePath"/>,
    /// creating or overwriting the file.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="storybook"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is null or whitespace.</exception>
    public void Save(Storybook storybook, string filePath)
    {
        ArgumentNullException.ThrowIfNull(storybook);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(storybook, JsonOptions);
        File.WriteAllText(filePath, json);
    }
}

