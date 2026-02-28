# Player Inventory

`ChagrinFalls.Backend.Systems.PlayerInventory`

Manages the set of items the player has collected. Item IDs are compared case-insensitively. Accessed via `GameState.Inventory`.

---

## Methods

### `AddItem`
```csharp
public void AddItem(string itemId)
```
Adds an item to the inventory. Throws `ArgumentException` if `itemId` is null or whitespace. Adding an item that is already present is a no-op.

### `RemoveItem`
```csharp
public bool RemoveItem(string itemId)
```
Removes an item. Returns `true` if the item was found and removed, `false` otherwise.

### `HasItem`
```csharp
public bool HasItem(string itemId)
```
Returns `true` if the inventory contains the specified item.

### `GetAllItems`
```csharp
public IReadOnlyCollection<string> GetAllItems()
```
Returns a snapshot of all item IDs currently held.

---

## Usage in conditions

The built-in `ItemInInventory` condition evaluator checks the inventory automatically. See the [Conditions](conditions.md) page for the parameter format.

---

## Example

```csharp
var inventory = gameState.Inventory;

inventory.AddItem("torn_letter");
inventory.AddItem("brass_key");

Console.WriteLine(inventory.HasItem("torn_letter")); // true
Console.WriteLine(inventory.HasItem("red_herring")); // false

inventory.RemoveItem("torn_letter");

foreach (var item in inventory.GetAllItems())
    Console.WriteLine(item); // brass_key
```

