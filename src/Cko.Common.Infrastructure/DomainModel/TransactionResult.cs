using System.Collections.Generic;

namespace Cko.Common.Infrastructure.DomainModel
{
    public class TransactionResult
    {
        public string TransactionId { get; set; }
        public IEnumerable<string> FromErrorReason { get; set; }
        public IEnumerable<string> ToErrorReason { get; set; }
    }
}
