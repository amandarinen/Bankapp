# BankApp – Blazor WebAssembly



A simple Blazor WebAssembly application for managing bank accounts, including deposits, withdrawals, transfers, and transaction history.



This project was developed as part of an introductory C# and .NET course.

It focuses on learning the fundamentals of Blazor, state management, and persistent data storage using localStorage — without external databases or backend services.





##### FEATURES



* Create, update, and delete bank accounts.
* Deposit and withdraw funds.
* Transfer between accounts.
* Add interest to Saving accounts.
* View transaction history with automatic filtering and sorting.
* Export accounts and transactions to JSON files.
* Import accounts from JSON files, with protection against duplicate account IDs.
* Lock/unlock the app using a PIN code.
* Local persistence using browser localStorage.





##### TECHNICAL DECISIONS



LocalStorage instead of a database.

Since this is an introductory course and we have not yet covered databases or backend APIs, all account data is saved locally in the browser using localStorage. This provides persistence between sessions while keeping the solution fully client-side.



Hardcoded PIN (1234).

The PIN code is hardcoded intentionally to avoid storing the password in the browser’s memory or localStorage.



Import validation.

When importing from JSON, accounts with duplicate IDs are automatically skipped to prevent data conflicts.



Console logging.

Important operations (such as saving, updating, and exporting accounts) are logged via Console.WriteLine() for easier debugging during development.





##### PREREQUISITES



.NET 8 SDK.

A modern web browser with localStorage support (Chrome, Edge, Firefox, Safari).





##### RUNNING THE APPLICATION



* Clone the repository: git clone https://github.com/amandarinen/Bankapp.git cd BlazorApp4
* Restore dependencies and build: dotnet restore dotnet build
* Run the application: dotnet run
* Open the browser at https://localhost:5001 (or the URL printed in the console).
* Enter the 4-digit PIN (1234) to unlock the app and start using it.





##### FUTURE IMPROVEMENTS



If the project were to be developed further, several enhancements could be implemented to make it more realistic. A natural next step would be to add user authentication, allowing multiple users to securely access their own accounts instead of sharing the same local data.



Another valuable improvement would be to include currency conversion, enabling users to view balances in different currencies.



In a more advanced version, transaction history and account data could be stored in a backend database instead of browser localStorage. This would make the application more robust, secure, and scalable.



Finally, instead of using simple Console.WriteLine() statements, logging could be handled through the ILogger interface. This would allow for structured logging, configurable log levels, and better debugging support in production scenarios.



