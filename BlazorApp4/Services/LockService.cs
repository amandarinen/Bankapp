public class LockService
{
    public bool IsLocked { get; private set; } = true;

    public event Action? OnLockStateChanged;

    public void Unlock(string pin)
    {
        if (pin == "1234")
        {
            IsLocked = false;
            OnLockStateChanged?.Invoke();
        }
    }

    public void Lock()
    {
        IsLocked = true;
        OnLockStateChanged?.Invoke();
    }
}
