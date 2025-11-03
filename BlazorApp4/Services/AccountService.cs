namespace BlazorApp4.Services
{
    /// <summary>
    /// Service responsible for managing bank accounts
    /// </summary>
    public class AccountService : IAccountService
    {
        private const string StorageKey = "BlazorApp4.accounts";
        private readonly List<BankAccount> _accounts = new();
        private readonly IStorageService _storageService;

        private bool isLoaded;

        /// <summary>
        /// Initializes a new instance of the AccountService.
        /// </summary>
        /// <param name="storageService">Injected storage service for persistence.</param>
        public AccountService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        /// <summary>
        /// Makes sure accounts are loaded from storage before use.
        /// </summary>
        public async Task EnsureLoadedAsync()
        {
            if (isLoaded)
                return;

            await IsInitialized();
            isLoaded = true;
            Console.WriteLine("[AccountService] Accounts loaded.");
        }

        /// <summary>
        /// Loads accounts from local storage if available.
        /// </summary>
        private async Task IsInitialized()
        {
            var fromStorage = await _storageService.GetItemAsync<List<BankAccount>>(StorageKey);
            if (fromStorage is { Count: > 0 })
                _accounts.AddRange(fromStorage);
            isLoaded = true;
        }

        /// <summary>
        /// Saves all accounts to local storage.
        /// </summary>
        private Task SaveAsync()
        {
            return _storageService.SetItemAsync(StorageKey, _accounts.OfType<BankAccount>().ToList());
        }

        /// <summary>
        /// Creates and saves a new bank account.
        /// </summary>
        public async Task<BankAccount> CreateAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance)
        {
            var account = new BankAccount(name, accountType, currency, initialBalance);

            if (accountType == AccountType.Savings)
                account.InterestRate = 0.02m;

            _accounts.Add(account);
            await SaveAsync();
            Console.WriteLine($"[AccountService] Account created: {account.Name} ({account.Id})");
            return account;
        }

        public List<BankAccount> GetAccounts() => _accounts.Cast<BankAccount>().ToList();

        public async Task SetAccounts(List<BankAccount> accounts)
        {
            _accounts.Clear();
            _accounts.AddRange(accounts);
            await _storageService.SetItemAsync(StorageKey, _accounts);
            Console.WriteLine("[AccountService] Accounts updated via SetAccounts.");
        }

        public async Task DeleteAccount(Guid Id)
        {
            var accountToRemove = _accounts.FirstOrDefault(a => a.Id == Id);
            if (accountToRemove != null)
            {
                _accounts.Remove(accountToRemove);
                await SaveAsync();
                Console.WriteLine($"[AccountService] Account deleted: {accountToRemove.Name} ({Id})");
            }
        }

        public async Task UpdateAccount(BankAccount updatedAccount)
        {
            var existing = _accounts.FirstOrDefault(a => a.Id == updatedAccount.Id);
            if (existing != null)
            {
                _accounts.Remove(existing);
                _accounts.Add(updatedAccount);
                await SaveAsync();
                Console.WriteLine($"[AccountService] Account updated: {updatedAccount.Name} ({updatedAccount.Id})");
            }
        }

        public async Task Transfer(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = _accounts.FirstOrDefault(a => a.Id == fromAccountId)
                ?? throw new KeyNotFoundException($"Account with ID {fromAccountId} not found.");
            var toAccount = _accounts.FirstOrDefault(a => a.Id == toAccountId)
                ?? throw new KeyNotFoundException($"Account with ID {toAccountId} not found.");

            if (fromAccount.Balance < amount)
                throw new InvalidOperationException("Insufficient funds.");
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

            fromAccount.TransferTo(toAccount, amount);
            await SaveAsync();
            Console.WriteLine($"[AccountService] Transfer: {amount} from {fromAccount.Name} to {toAccount.Name}");
        }

        public async Task DepositAsync(Guid accountId, decimal amount)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == accountId)
                ?? throw new KeyNotFoundException($"Account with ID {accountId} not found.");
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");

            account.Deposit(amount);
            await SaveAsync();
            Console.WriteLine($"[AccountService] Deposit: {amount} to {account.Name}");
        }

        public async Task WithdrawAsync(Guid accountId, decimal amount)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == accountId)
                ?? throw new KeyNotFoundException($"Account with ID {accountId} not found.");
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be positive.");
            if (account.Balance < amount)
                throw new InvalidOperationException("Insufficient balance.");

            account.Withdraw(amount);
            await SaveAsync();
            Console.WriteLine($"[AccountService] Withdraw: {amount} from {account.Name}");
        }

        public async Task ApplyInterestAsync()
        {
            foreach (var account in _accounts.Where(a =>
                     a.AccountType == AccountType.Savings &&
                     a.InterestRate.HasValue &&
                     a.InterestRate > 0))
            {
                account.ApplyInterest();
            }

            await SaveAsync();
            Console.WriteLine("[AccountService] Interest applied to savings accounts.");
        }
    }
}
