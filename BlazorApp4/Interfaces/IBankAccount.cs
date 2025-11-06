namespace BlazorApp4.Interfaces
{
    /// <summary>
    /// Defines the behavior and properties of a bank account.
    /// </summary>
    public interface IBankAccount
    {
        // Unique identifier for the account.
        Guid Id { get; }

        // The name of the account.
        string Name { get; }

        // The type of the account (Deposit or Savings).
        AccountType AccountType { get; }

        // The currency used for the account.
        CurrencyType Currency { get; }

        // The current balance of the account.
        decimal Balance { get; }

        // The last time the account was updated.
        DateTime LastUpdated { get; }

        // A list of all transactions related to the account.
        List<Transaction> Transactions { get; }

        // Withdraws a specified amount from the account.
        void Withdraw(decimal amount);

        // Deposits a specified amount into the account.
        void Deposit(decimal amount);

        // Transfers a specified amount to another account.
        void TransferTo(BankAccount toAccount, decimal amount);
    }
}
