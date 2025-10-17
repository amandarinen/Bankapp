
namespace BlazorApp4.Interfaces
{
    public interface IAccountService
    {
        Task<IBankAccount> CreateAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance);
        Task<List<IBankAccount>> GetAccounts();

        Task DeleteAccount(Guid Id);
        Task UpdateAccount(IBankAccount updatedAccount);

        Task Transfer(Guid fromAccountId, Guid toAccountId, decimal amount);

    }
}
