namespace Cko.PaymentGateway.Infrastructure.DomainModel
{
    public enum TransactionStatus
    {
        BankDenied = -2,
        Failed = -1,
        Pending = 0,
        Success = 1
    }
}
