namespace Orders.Contracts.Inventory;

public class InventoryAllocationFailed
{
    public Guid OrderId { get; set; }
    public string Reason { get; set; }
}