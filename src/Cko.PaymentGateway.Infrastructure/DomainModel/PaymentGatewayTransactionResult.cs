using Cko.Common.Infrastructure.DomainModel;
using System;
using System.Linq;

namespace Cko.PaymentGateway.Infrastructure.DomainModel
{
    public class PaymentGatewayTransactionResult
    {
        public string TransactionId { get; set; }
        public PaymentGatewayBankTransactionResult BankTransactionResult { get; set; }
        public TransactionStatus TransactionStatus
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(BankTransactionResult.ConnectionError))
                {
                    return TransactionStatus.Failed;
                }
                else if (BankTransactionResult.BankTransactionResult.FromErrorReasons.Any() ||
                         BankTransactionResult.BankTransactionResult.ToErrorReasons.Any())
                {
                    return TransactionStatus.BankDenied;
                }
                return TransactionStatus.Success;
            }
        }
        public string TransactionStatusText => TransactionStatus.ToString();
        public DateTime TransactionDateUtc { get; set; }
        public Transaction OriginalTransaction { get; set; }

    }
}
