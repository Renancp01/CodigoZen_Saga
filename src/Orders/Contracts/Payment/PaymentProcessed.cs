namespace Orders.Contracts.Payment;

public class PaymentProcessed
{
    public Guid OrderId { get; set; }
    public bool Success { get; set; }
    public string FailureReason { get; set; }
}