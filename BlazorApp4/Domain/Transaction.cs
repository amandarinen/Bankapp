using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorApp4.Domain
{
    /// <summary>
    /// The available types of transactions
    /// </summary>
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        TransferIn,
        TransferOut,
        Interest
    }

    /// <summary>
    /// Represents a single transaction made on a bank account
    /// </summary>
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
