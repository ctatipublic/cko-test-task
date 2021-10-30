using Cko.Common.Infrastructure.DomainModel;
using System;

namespace Cko.PaymentGateway.Infrastructure.DomainModel
{
    public class PaymentGatewayTransaction : Transaction
    {
        public DateTime TransactionDateTimeUtc { get; set; }
        public TransactionStatus TransactionStatus { get; set; }
    }
}
