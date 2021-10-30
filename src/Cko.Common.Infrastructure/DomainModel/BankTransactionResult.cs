using System.Collections.Generic;

namespace Cko.Common.Infrastructure.DomainModel
{
    public class BankTransactionResult
    {
        public string TransactionId { get; set; }
        public IEnumerable<string> FromErrorReasons { get; set; }
        public IEnumerable<string> ToErrorReasons { get; set; }
    }
}
