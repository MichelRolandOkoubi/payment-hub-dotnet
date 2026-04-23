using MediatR;
using FluentValidation;
using BuildingBlocks.Messaging;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Commands;

public record ProcessPaymentCommand(
    Guid MerchantId,
    string Iban,
    decimal Amount,
    string Currency,
    string? Description
) : IRequest<PaymentResult>;

public record PaymentResult(Guid PaymentId, PaymentStatus Status);

public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, PaymentResult>
{
    private readonly IKafkaProducer _kafka;
    // Simulation du Repository et de l'Idempotence pour l'exemple complet
    
    public ProcessPaymentHandler(IKafkaProducer kafka)
    {
        _kafka = kafka;
    }

    public async Task<PaymentResult> Handle(ProcessPaymentCommand request, CancellationToken ct)
    {
        // 1. Logique simplifiée pour l'exemple
        var payment = Payment.Create(
            request.MerchantId,
            request.Iban,
            new Money(request.Amount, request.Currency),
            request.Description
        );

        // 2. Publication sur Kafka pour FraudDetection
        await _kafka.ProduceAsync("payment.initiated", new 
        {
            PaymentId = payment.Id,
            MerchantId = payment.MerchantId,
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency,
            Timestamp = DateTime.UtcNow
        });

        return new PaymentResult(payment.Id, payment.Status);
    }
}
