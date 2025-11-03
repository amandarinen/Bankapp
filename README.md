A simple Blazor WebAssembly application for managing bank accounts, including deposits, withdrawals, transfers, and transaction history.

FEATURES

Create, update, and delete bank accounts.
Deposit and withdraw funds.
Transfer between accounts.
Manually add interest to Saving accounts.
View transaction history with automatic filtering and sorting.
Export accounts and transactions to JSON files.
Import accounts from JSON files, with protection against duplicate account IDs.
Lock/unlock the app using a PIN code for privacy.
Local persistence using browser localStorage.


TECHNICAL DECISIONS

In AccountService: .OfType<BankAccount>() has been removed from Transfer(), DepositAsync() and WithdrawAsync() because _accounts is already a List<BankAccount> so casting was redundant.

Automatic transaction filtering: In TransactionHistory, filtering occurs immediately when the user changes date ranges or transaction type, giving instant feedback without extra button clicks.

Import validation: Accounts with duplicate IDs cannot be imported. This prevents confusion about which account data is saved and ensures a consistent state.

Console logging: Key events such as saving accounts or exporting data are logged using Console.WriteLine, which makes debugging easier.


PREREQUISITES

.NET 8 SDK
Supported browser with localStorage (modern Chrome, Edge, Firefox, Safari)


RUNNING THE APPLICATION

1. Clone the repository:
  git clone https://github.com/yourusername/BlazorApp4.git
  cd BlazorApp4
2. Restore dependencies and build:
  dotnet restore
  dotnet build
3. Run the application:
   dotnet run
4. Open the browser at https://localhost:5001
 (or the URL printed in the console).
5. Enter the 4-digit PIN (1234) to unlock the app and start using it.


FUTURE IMPROVEMENTS

Add user authentication for multi-user support.
Add currency conversion and reporting features.
Persist transaction history to a backend database instead of just localStorage.
Use ILogger for more structured logging and optional log levels.
