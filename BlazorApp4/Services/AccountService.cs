
namespace BlazorApp4.Services
{
    public class AccountService : IAccountService
    {
        private const string StorageKey = "BlazorApp4.accounts";
        private readonly List<IBankAccount> _accounts = new();
        private readonly IStorageService _storageService;

        private bool isLoaded;

        public AccountService(IStorageService storageService) => _storageService = storageService;

        private async Task IsInitialized()
        {
            if (isLoaded)
            {
                return;
            }
            var fromStorage = await _storageService.GetItemAsync<List<BankAccount>>(StorageKey);
            _accounts.Clear();
            if (fromStorage is { Count: > 0 })
                _accounts.AddRange(fromStorage);
            isLoaded = true;
        }

        private Task SaveAsync() => _storageService.SetItemAsync(StorageKey, _accounts);
        

        public async Task <IBankAccount> CreateAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance)
        {
            await IsInitialized();
            var account = new BankAccount(name, accountType, currency, initialBalance);
            _accounts.Add(account);
            await SaveAsync();
            return account;
        }

        public async Task<List<IBankAccount>> GetAccounts()
        {
            await IsInitialized();
            return _accounts.Cast<IBankAccount>().ToList();
        }

        public async Task DeleteAccount(Guid Id)
        {
            await IsInitialized();

            var accountToRemove = _accounts.FirstOrDefault(account => account.Id == Id);

            if (accountToRemove is not null)
            {
                _accounts.Remove(accountToRemove);
                await SaveAsync();
                
            }
        }

        public async Task UpdateAccount(IBankAccount updatedAccount)
        {
            await IsInitialized();

            var existing = _accounts.FirstOrDefault(account => account.Id == updatedAccount.Id);
            if (existing != null)
           {
                _accounts.Remove(existing);
                _accounts.Add(updatedAccount);
                await SaveAsync();
            }
        }

        public async Task Transfer(Guid fromAccountId,  Guid toAccountId, decimal amount)
        {
            var fromAccount = _accounts.OfType<BankAccount>().FirstOrDefault(x => x.Id == fromAccountId)
                ?? throw new KeyNotFoundException($"Account with ID {fromAccountId} not found.");

            var toAccount = _accounts.OfType<BankAccount>().FirstOrDefault(x => x.Id == toAccountId)
                ?? throw new KeyNotFoundException($"Account with ID {fromAccountId} not found.");

            fromAccount.TransferTo(toAccount, amount);
        }

    }
}

