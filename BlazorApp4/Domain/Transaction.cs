namespace BlazorApp4.Domain
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        Transfer
    }

    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; } = DateTime.Now;
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal BalanceAfterTransaction { get; set; }
        public Guid? FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
    }
}
