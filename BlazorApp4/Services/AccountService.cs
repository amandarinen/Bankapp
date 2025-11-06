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
            await ApplyInterestAsync();
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
        /// Creates a new bank account, validates input, assigns interest rate to savings accounts.
        /// </summary>
        /// <param name="name">Account name.</param>
        /// <param name="accountType">Type of account.</param>
        /// <param name="currency">Currency used by the account.</param>
        /// <param name="initialBalance">Initial deposited amount.</param>
        /// <returns>The newly created account.</returns>
        public async Task<BankAccount> CreateAccount(string name, AccountType accountType, CurrencyType currency, decimal initialBalance)
        {
            var account = new BankAccount(name, accountType, currency, initialBalance);

            if (accountType == AccountType.Savings)
                account.InterestRate = 0.02m;
            
            if (string.IsNullOrWhiteSpace(name))
                            throw new InvalidOperationException("Account name can not be empty.");
            if (initialBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Amount must be positive.");

            _accounts.Add(account);
            await SaveAsync();
            Console.WriteLine($"[AccountService] Account created: {account.Name} ({account.Id})");
            return account;
        }

        /// <summary>
        /// Returns a list of all bank accounts.
        /// </summary>
        public List<BankAccount> GetAccounts() => _accounts.Cast<BankAccount>().ToList();

        /// <summary>
        /// Replaces the current account list with a new one and saves it to storage.
        /// Also ensures imported savings accounts receive their default interest rate if missing.
        /// </summary>
        public async Task SetAccounts(List<BankAccount> accounts)
        {
            foreach (var account in accounts)
            {
                // Ensure all savings accounts have a default interest rate
                if (account.AccountType == AccountType.Savings &&
                    (!account.InterestRate.HasValue || account.InterestRate == 0))
                {
                    account.InterestRate = 0.02m;
                }
            }

            _accounts.Clear();
            _accounts.AddRange(accounts);
            await _storageService.SetItemAsync(StorageKey, _accounts);
            Console.WriteLine("[AccountService] Accounts updated via SetAccounts (interest ensured for savings accounts).");
        }


        /// <summary>
        /// Deletes an account from the system by ID.
        /// </summary>
        /// <param name="Id">The unique ID of the account to delete.</param>
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

        /// <summary>
        /// Updates an existing account and saves the changes to storage.
        /// </summary>
        /// <param name="updatedAccount">The updated account data.</param>
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

        /// <summary>
        /// Transfers funds between two accounts after validating balances and input.
        /// </summary>
        /// <param name="fromAccountId">Source account ID.</param>
        /// <param name="toAccountId">Destination account ID.</param>
        /// <param name="amount">Amount to transfer.</param>
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

        /// <summary>
        /// Deposits a specified amount into a given account.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="amount">The amount to deposit.</param>
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

        /// <summary>
        /// Withdraws a specified amount from a given account, if sufficient funds exist.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="amount">The amount to withdraw.</param>
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

        /// <summary>
        /// Applies interest to all savings accounts, automatically when accounts are loaded.
        /// </summary>
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
