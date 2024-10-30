namespace Orders.Contracts.Inventory;

public class InventoryAllocationRequested
{
    public Guid OrderId { get; set; }
    public string ItemNumber { get; set; }
    public int Quantity { get; set; }
}