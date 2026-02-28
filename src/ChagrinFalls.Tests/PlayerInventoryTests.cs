using ChagrinFalls.Backend.Systems;

namespace ChagrinFalls.Tests;

public class PlayerInventoryTests
{
    [Fact]
    public void HasItem_ReturnsFalse_WhenInventoryIsEmpty()
    {
        var inventory = new PlayerInventory();
        Assert.False(inventory.HasItem("rusty_key"));
    }

    [Fact]
    public void HasItem_ReturnsTrue_AfterAddingItem()
    {
        var inventory = new PlayerInventory();
        inventory.AddItem("rusty_key");
        Assert.True(inventory.HasItem("rusty_key"));
    }

    [Fact]
    public void HasItem_IsCaseInsensitive()
    {
        var inventory = new PlayerInventory();
        inventory.AddItem("Rusty_Key");
        Assert.True(inventory.HasItem("rusty_key"));
        Assert.True(inventory.HasItem("RUSTY_KEY"));
    }

    [Fact]
    public void HasItem_ReturnsFalse_ForNullOrWhitespace()
    {
        var inventory = new PlayerInventory();
        Assert.False(inventory.HasItem(null!));
        Assert.False(inventory.HasItem("   "));
    }

    [Fact]
    public void AddItem_Throws_ForNullOrWhitespace()
    {
        var inventory = new PlayerInventory();
        Assert.Throws<ArgumentException>(() => inventory.AddItem(null!));
        Assert.Throws<ArgumentException>(() => inventory.AddItem("   "));
    }

    [Fact]
    public void RemoveItem_ReturnsTrue_WhenItemExisted()
    {
        var inventory = new PlayerInventory();
        inventory.AddItem("rusty_key");
        Assert.True(inventory.RemoveItem("rusty_key"));
        Assert.False(inventory.HasItem("rusty_key"));
    }

    [Fact]
    public void RemoveItem_ReturnsFalse_WhenItemNotPresent()
    {
        var inventory = new PlayerInventory();
        Assert.False(inventory.RemoveItem("rusty_key"));
    }

    [Fact]
    public void GetAllItems_ReturnsAllItems()
    {
        var inventory = new PlayerInventory();
        inventory.AddItem("rusty_key");
        inventory.AddItem("candle");
        Assert.Equal(2, inventory.GetAllItems().Count);
    }
}
