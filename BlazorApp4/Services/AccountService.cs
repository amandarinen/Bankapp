
namespace BlazorApp4.Services
{
    public class AccountService : IAccountService
    {
        private const string StorageKey = "BlazorApp4.accounts";
        private readonly List<BankAccount> _accounts = new();
        private readonly IStorageService _storageService;

        private bool isLoaded;

        public AccountService(IStorageService storageService)
        {
            _storageService = storageService;
            
        }

        public async Task EnsureLoadedAsync()
        {
            if (isLoaded)
            {
                return;
            }
            await IsInitialized();
            isLoaded = true;
        }

        private async Task IsInitialized()
        {
            var fromStorage = await _storageService.GetItemAsync<List<BankAccount>>(StorageKey);
            if (fromStorage is { Count: > 0 })
                _accounts.AddRange(fromStorage);
            isLoaded = true;
        }

        private Task SaveAsync() => _storageService.SetItemAsync(StorageKey, _accounts.OfType<BankAccount>().ToList());

        public async Task<BankAccount> CreateAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance)
        {
            var account = new BankAccount(name, accountType, currency, initialBalance);
            _accounts.Add(account);
           await SaveAsync();
            return account;
        }

        public List<BankAccount> GetAccounts()
        {
            return _accounts.Cast<BankAccount>().ToList();
        }

        public async Task DeleteAccount(Guid Id)
        {
            var accountToRemove = _accounts.FirstOrDefault(account => account.Id == Id);

            if (accountToRemove is not null)
            {
                _accounts.Remove(accountToRemove);
               await SaveAsync();  
            }
        }

        public async Task UpdateAccount(BankAccount updatedAccount)
        {
            var existing = _accounts.FirstOrDefault(account => account.Id == updatedAccount.Id);
            if (existing != null)
           {
                _accounts.Remove(existing);
                _accounts.Add(updatedAccount);
                await SaveAsync();
            }
        }

        public async Task Transfer(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = _accounts.OfType<BankAccount>().FirstOrDefault(a => a.Id == fromAccountId)
            ?? throw new KeyNotFoundException($"Account with ID {fromAccountId} not found.");
            var toAccount = _accounts.OfType<BankAccount>().FirstOrDefault(a => a.Id == toAccountId)
            ?? throw new KeyNotFoundException($"Account with ID {toAccountId} not found.");

            if (fromAccount.Balance < amount)
            {
                throw new InvalidOperationException("Otillräckliga medel på från-kontot.");
            }
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Beloppet måste vara positivt.");
            }

            fromAccount.TransferTo(toAccount, amount);
            await SaveAsync();
        }

        public async Task DepositAsync(Guid accountId, decimal amount)
        {
            var account = _accounts.OfType<BankAccount>().FirstOrDefault(a => a.Id == accountId)
                ?? throw new KeyNotFoundException($"Konto med ID {accountId} hittades inte.");

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Beloppet måste vara större än 0.");
            }

            account.Deposit(amount);
            await SaveAsync();
        }

        public async Task WithdrawAsync(Guid accountId, decimal amount)
        {
            var account = _accounts.OfType<BankAccount>().FirstOrDefault(a => a.Id == accountId)
                ?? throw new KeyNotFoundException($"Konto med ID {accountId} hittades inte.");

            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Beloppet måste vara större än 0.");
            }

            if (account.Balance < amount)
            {
                throw new InvalidOperationException("Otillräckligt saldo.");
            }

            account.Withdraw(amount);
            await SaveAsync();
        }
    }
}

