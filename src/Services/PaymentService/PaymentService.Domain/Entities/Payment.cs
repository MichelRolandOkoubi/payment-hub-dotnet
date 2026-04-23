namespace PaymentService.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid MerchantId { get; private set; }
    public string Iban { get; private set; }
    public Money Amount { get; private set; }
    public string? Description { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Payment() { } // EF Core

    public static Payment Create(Guid merchantId, string iban, Money amount, string? description)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            Iban = iban,
            Amount = amount,
            Description = description,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public record Money(decimal Amount, string Currency);

public enum PaymentStatus
{
    Pending,
    Authorized,
    Completed,
    Failed,
    DeclinedByFraud
}
