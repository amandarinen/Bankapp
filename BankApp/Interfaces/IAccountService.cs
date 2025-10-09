namespace BankApp.Interfaces
{
    public interface IAccountService
    {
        IBankAccount CreateAccount(string name, string currency, decimal initialBalance); //account typ? vanligt konto eller sparkonto?
        List<IBankAccount> GetAccounts();
    }
}
