namespace BlazorApp4.Domain
{
    /// <summary>
    /// The available types of transactions
    /// </summary>
    public enum TransactionType
    {
        Deposit,
        Withdrawal,
        TransferIn,
        TransferOut,
        Interest
    }

    /// <summary>
    /// The available types of currency
    /// </summary>
    public enum CurrencyType
    {
        SEK
    }

    /// <summary>
    /// The available types of bank accounts
    /// </summary>
    public enum AccountType
    {
        Savings,
        Deposit
    }
}