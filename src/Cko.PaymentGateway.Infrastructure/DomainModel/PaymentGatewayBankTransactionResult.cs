using Cko.Common.Infrastructure.DomainModel;

namespace Cko.PaymentGateway.Infrastructure.DomainModel
{
    public class PaymentGatewayBankTransactionResult 
    {
        public BankTransactionResult BankTransactionResult { get; set; }
        public string ConnectionError { get; set; }
    }
}
