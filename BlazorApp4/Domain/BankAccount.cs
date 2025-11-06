
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorApp4.Domain
    
{
    /// <summary>
    /// Bankkonto domain, hanterar transaktioner, överföringar och sparar properties kopplade till bankkontot
    /// </summary>
    public class BankAccount : IBankAccount
    {
        // Properties
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Name { get; private set; }
        public AccountType AccountType { get; private set; }
        public CurrencyType Currency { get; private set; }
        public decimal Balance { get; private set; }
        public DateTime LastUpdated { get; private set; }
        public decimal? InterestRate { get; set; } 

        // List of all transactions for this account
        public readonly List<Transaction> _transaction = new();
        public List<Transaction> Transactions => _transaction;

        /// <summary>
        /// Creates a new bank account with the specified information
        /// </summary>
        public BankAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance)
        {
            Name = name;
            AccountType = accountType;
            Currency = currency;
            Balance = initialBalance;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        /// JSON constructor used for deserialization of account data.
        /// </summary>
        [JsonConstructor]
        public BankAccount(Guid id, string name, AccountType accountType, CurrencyType currency, decimal balance, DateTime lastUpdated, List<Transaction>? transactions = null, decimal? interestRate = null)
        {
            Id = id;
            Name = name;
            AccountType = accountType;
            Currency = currency;
            Balance = balance;
            LastUpdated = lastUpdated;
            InterestRate = interestRate ?? 0m;

            if (transactions != null)
                _transaction = transactions;
        }

        /// <summary>
        /// Transfers a specific amount from one account to another
        /// </summary>
        /// <param name="toAccount">Which account to transfer to</param>
        /// <param name="amount">The amount to transfer</param>
        public void TransferTo(BankAccount toAccount, decimal amount)
        {
            // Withdraw from this account
            Balance -= amount;
            LastUpdated = DateTime.UtcNow;
            _transaction.Add(new Transaction
            {
                transactionType = TransactionType.TransferOut,
                Amount = amount,
                BalanceAfterTransaction = Balance,
                FromAccountId = Id,
                ToAccountId = toAccount.Id,
                TimeStamp = DateTime.UtcNow
            });

            // Deposit to this account
            toAccount.Balance += amount;
            toAccount.LastUpdated = DateTime.UtcNow;
            toAccount._transaction.Add(new Transaction
            {
                transactionType = TransactionType.TransferIn,
                Amount = amount,
                BalanceAfterTransaction = toAccount.Balance,
                FromAccountId = Id,
                ToAccountId = toAccount.Id,
                TimeStamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Deposits a specific amount into the account
        /// </summary>
        /// <param name="amount">The amount to deposit</param>
        /// <exception cref="ArgumentException">Thrown if the amount is less than zero</exception>
        public void Deposit(decimal amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("The amount must be greater than 0!");
            }

            Balance += amount;
            LastUpdated = DateTime.UtcNow;
            
            _transaction.Add(new Transaction
            {
                transactionType = TransactionType.Deposit,
                Amount = amount,
                BalanceAfterTransaction = Balance,
                FromAccountId = Id
            });
        }

        /// <summary>
        /// Withdraws a specific amount from the account.
        /// </summary>
        /// <param name="amount">The amount to withdraw.</param>
        /// <exception cref="ArgumentException">Thrown if the amount is less than zero.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the balance is insufficient.</exception>
        public void Withdraw(decimal amount)
        {
            if (amount < 0)
            {
                throw new ValidationException("The amount must be greater than 0!");
            }

            if (Balance < amount)
            {
                throw new InvalidOperationException("Insufficient balance!");
            }

            Balance -= amount;
            LastUpdated = DateTime.UtcNow;

            _transaction.Add(new Transaction
            {
                transactionType = TransactionType.Withdrawal,
                Amount = amount,
                BalanceAfterTransaction = Balance,
                FromAccountId = Id
            });
        }

        /// <summary>
        /// Applies interest to the balance if the account is a savings account
        /// </summary>
        public void ApplyInterest()
        {
            if (AccountType != AccountType.Savings)
            {
                return;
            }

            if (InterestRate.GetValueOrDefault() <= 0)
            {
                return;
            }

            var daysElapsed = (DateTime.UtcNow - LastUpdated).TotalDays;

            if (daysElapsed < 1)
            {
                daysElapsed = 1;
            }

            decimal dailyRate = InterestRate.GetValueOrDefault() / 365m;
            decimal interestAmount = Balance * dailyRate * (decimal)daysElapsed;

            Balance += Math.Round(interestAmount, 2);
            LastUpdated = DateTime.UtcNow;

            _transaction.Add(new Transaction
            {
                transactionType = TransactionType.Interest,
                Amount = Math.Round(interestAmount, 2),
                BalanceAfterTransaction = Balance,
                TimeStamp = DateTime.UtcNow
            });

            Console.WriteLine($"[BankAccount] Interest applied to {Name}");
        }
    }
}