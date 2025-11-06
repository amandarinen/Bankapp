namespace BlazorApp4.Interfaces
{
    /// <summary>
    /// Defines methods for creating, managing, and updating bank accounts.
    /// </summary>
    public interface IAccountService
    {
        // Creates a new bank account.
        Task<BankAccount> CreateAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance);

        // Returns all stored bank accounts.
        List<BankAccount> GetAccounts();

        // Replaces the current account list with a new one and saves it.
        Task SetAccounts(List<BankAccount> accounts);

        // Deletes an account by its ID.
        Task DeleteAccount(Guid Id);

        // Updates an existing account with new information.
        Task UpdateAccount(BankAccount updatedAccount);

        // Transfers a specific amount between two accounts.
        Task Transfer(Guid fromAccountId, Guid toAccountId, decimal amount);

        // Ensures that account data is loaded from storage before use.
        Task EnsureLoadedAsync();

        // Deposits a specific amount into an account.
        Task DepositAsync(Guid accountId, decimal amount);

        // Withdraws a specific amount from an account.
        Task WithdrawAsync(Guid accountId, decimal amount);

        // Applies interest to all savings accounts.
        Task ApplyInterestAsync();

        //Event triggered whenever the state of the accounts changes
        event Action? StateChanged;

        //Starts an automatic daily process that applies interest to all savings accounts
        void AutoApplyInterest();
    }
}