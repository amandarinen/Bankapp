public class LockService
{
    /// <summary>
    /// Indicates whether the app is currently locked.
    /// </summary>
    public bool IsLocked { get; private set; } = true;

    /// <summary>
    /// Event that is triggered when the lock state changes.
    /// </summary>
    public event Action? OnLockStateChanged;

    /// <summary>
    /// Unlocks the app if the correct PIN is entered.
    /// </summary>
    /// <param name="pin">The PIN code used to unlock.</param>
    public void Unlock(string pin)
    {
        if (pin == "1234")
        {
            IsLocked = false;
            OnLockStateChanged?.Invoke();
            Console.WriteLine("App unlocked successfully.");
        }
        else
        {
            Console.WriteLine("Failed unlock attempt with incorrect PIN.");
        }
    }

    /// <summary>
    /// Locks the app and triggers the state change event.
    /// </summary>
    public void Lock()
    {
        IsLocked = true;
        OnLockStateChanged?.Invoke();
        Console.WriteLine("App locked.");
    }
}
