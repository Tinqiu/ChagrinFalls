using System.Text.Json;
using System.Text.Json.Serialization;
using ChagrinFalls.Backend.Models;

namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Discovers and deserialises <see cref="Storybook"/> instances from a directory of JSON files.
/// Each <c>*.json</c> file in the target directory is expected to contain one storybook.
/// </summary>
public class StorybookLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    /// <summary>
    /// Loads all storybooks found in <paramref name="directory"/>.
    /// Files that cannot be parsed are skipped; errors are reported via <paramref name="onError"/>.
    /// </summary>
    /// <param name="directory">Absolute path to the directory containing storybook JSON files.</param>
    /// <param name="onError">Optional callback invoked with the file path and exception for any file that fails to load.</param>
    /// <returns>All successfully loaded storybooks.</returns>
    public IReadOnlyList<Storybook> LoadAll(string directory, Action<string, Exception>? onError = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);

        if (!Directory.Exists(directory))
            return [];

        var results = new List<Storybook>();

        foreach (var file in Directory.EnumerateFiles(directory, "*.json"))
        {
            try
            {
                var storybook = LoadFile(file);
                results.Add(storybook);
            }
            catch (Exception ex)
            {
                onError?.Invoke(file, ex);
            }
        }

        return results;
    }

    /// <summary>
    /// Loads a single storybook from the specified JSON file path.
    /// </summary>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="JsonException">The file is not valid storybook JSON.</exception>
    /// <exception cref="InvalidOperationException">The deserialised storybook has a missing or empty Id.</exception>
    public Storybook LoadFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Storybook file not found: '{filePath}'.", filePath);

        var json = File.ReadAllText(filePath);
        var storybook = JsonSerializer.Deserialize<Storybook>(json, JsonOptions)
            ?? throw new JsonException($"File '{filePath}' deserialised to null.");

        if (string.IsNullOrWhiteSpace(storybook.Id))
            throw new InvalidOperationException($"Storybook in '{filePath}' has a missing or empty Id.");

        return storybook;
    }
}

