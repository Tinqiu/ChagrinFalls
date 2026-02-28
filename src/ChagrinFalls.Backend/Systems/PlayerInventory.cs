namespace ChagrinFalls.Backend.Systems;

/// <summary>
/// Manages the items the player has collected throughout the game.
/// Used to evaluate <c>ItemInInventory</c> conditions during conversations.
/// </summary>
public class PlayerInventory
{
    private readonly HashSet<string> _items = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Adds an item to the inventory.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item to add.</param>
    public void AddItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            throw new ArgumentException("Item ID cannot be null or whitespace.", nameof(itemId));

        _items.Add(itemId);
    }

    /// <summary>
    /// Removes an item from the inventory.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item to remove.</param>
    /// <returns><c>true</c> if the item was found and removed; <c>false</c> otherwise.</returns>
    public bool RemoveItem(string? itemId) =>
        !string.IsNullOrWhiteSpace(itemId) && _items.Remove(itemId);

    /// <summary>
    /// Returns whether the specified item is in the inventory.
    /// </summary>
    /// <param name="itemId">The unique identifier of the item to check.</param>
    public bool HasItem(string? itemId) =>
        !string.IsNullOrWhiteSpace(itemId) && _items.Contains(itemId);

    /// <summary>
    /// Returns all item IDs currently held in the inventory.
    /// </summary>
    public IReadOnlyCollection<string> GetAllItems() =>
        _items.ToList().AsReadOnly();
}
