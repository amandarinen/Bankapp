using System.ComponentModel.DataAnnotations;

namespace BlazorApp4.Domain
{
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        TransferIn,
        TransferOut,
        Interest
    }

    public class Transaction
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
        public TransactionType transactionType { get; set; }
        public decimal Amount { get; set; }
        public CurrencyType Currency { get; set; }
        public decimal BalanceAfterTransaction { get; set; }
        public Guid? FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
    }
}
