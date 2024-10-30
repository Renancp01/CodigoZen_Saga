namespace Orders.Contracts.Payment;

public class ProcessPayment
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
}