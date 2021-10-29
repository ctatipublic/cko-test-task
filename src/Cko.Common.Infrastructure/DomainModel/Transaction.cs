namespace Cko.Common.Infrastructure.DomainModel
{
    public class Transaction
    {
        public CardDetails From { get; set; }
        public CardDetails To { get; set; } 
        public decimal Amount { get; set; }
    }
}
