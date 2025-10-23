
namespace BlazorApp4.Interfaces
{
    public interface IAccountService
    {
        Task<BankAccount> CreateAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance);
        List<BankAccount> GetAccounts();

        Task DeleteAccount(Guid Id);
        Task UpdateAccount(BankAccount updatedAccount);

        Task Transfer(Guid fromAccountId, Guid toAccountId, decimal amount);

    }
}
