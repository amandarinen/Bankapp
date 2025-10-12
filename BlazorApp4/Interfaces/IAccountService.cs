
namespace BlazorApp4.Interfaces
{
    public interface IAccountService
    {
        IBankAccount CreateAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance);
        List<IBankAccount> GetAccounts();
    }
}
