
namespace BlazorApp4.Services
{
    public class AccountService : IAccountService
    {
        private readonly List<IBankAccount> _accounts = new();
        public IBankAccount CreateAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance)
        {
            var account = new BankAccount(name, accountType, currency, initialBalance);
            _accounts.Add(account);
            return account;
        }

        public List<IBankAccount> GetAccounts() => _accounts;

    }
}

